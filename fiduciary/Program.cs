using fiduciary.Contracts;
using fiduciary.Data;
using fiduciary.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlite("Data Source=knowledgebase.db");  
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger(); // Enable Swagger middleware
app.UseSwaggerUI(); // Enable Swagger UI

// Define API endpoints
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "All Good", 
        app = "Fiduciary Knowledge Base API",
        timestamp = DateTime.UtcNow
    });
});

app.MapGet("/api/articles", async (
    AppDbContext db,
    string? search, 
    string? category,
    string? tags) =>
    {
    var query  = db.KnowledgeArticle.AsQueryable(); // Start with the base query

    if (!string.IsNullOrWhiteSpace(search)
    {
        var normalisedSearch = search.Trim().ToLower(); // Normalize search term

        query = query.Where(article => 
            article.Title.ToLower().Contains(normalisedSearch) ||
            article.Summary.ToLower().Contains(normalisedSearch) ||
            article.Content.ToLower().Contains(normalisedSearch) ||
            
    }

    if (!string.IsNullOrWhiteSpace(category))
        {
            var normalisedCategory = category.Trim().ToLower();

            query = query.Where(article => article.Category.ToLower() == normalisedCategory);
        }
    
    if (!string.IsNullOrWhiteSpace(tags))
        {
            var normalisedTags = tags.Trim().ToLower().Split(',').Select(t => t.Trim()); // Normalize and split tags into an array

            query = query.Where(article => 
                normalisedTags.Any(tag => article.Tags.ToLower().Contains(tag)));
        }

    var articles = await query
        .OrderBy(article => article.Title) 
        .Select(article => new
        {
            article.Id,
            article.Title,
            article.Summary,
            article.Category,
            article.Tags,
            article.Source,
            article.CreatedAtUtc,
            article.UpdatedAtUtc
        })
        .ToListAsync();
    
    return Results.Ok(articles);
}
);


// get specifc article
app.MapGet("/api/articles/{id}", async (int id, AppDbContext db) =>
{
    var article = await db.KnowledgeArticle.FindAsync(id);

    return article is null
        ? Results.NotFound(new { message = $"Article with ID {id} was not found"})
        : Results.Ok(new
        {
            article.Id,
            article.Title,
            article.Summary,
            article.Content,
            article.Category,
            article.Tags,
            article.Source,
            article.CreatedAtUtc,
            article.UpdatedAtUtc
        });
});

app.MapPost("/api/articles", async (CreatedArticleRequest request, AppDbContext db) =>
{
    var validationError = ValidateArticleInput(
        request.Title,
        request.Summary,
        request.Content,
        request.Category,
        request.Tags,
        request.Source
    );

    if (validationError is not null)
    {
        return Results.BadRequest(new { message = validationError });
    }

    // Create and save the new article
    var article = new KnowledgeArticle
    {
        Title = request.Title.Trim(),
        Summary = request.Summary.Trim(),
        Content = request.Content.Trim(),
        Category = request.Category.Trim(),
        Tags = NormaliseTags(request.Tags),
        Source = string.IsNullOrWhiteSpace(request.Source) ? null : request.Source.Trim(),
    };

    db.KnowledgeArticle.Add(article); //    Add the new article to the database context
    await db.SaveChangesAsync(); // Save changes to the database

    // Return the created article with a 201 Created status
    return Results.Created($"/api/articles/{article.Id}", new
    {
        article.Id,
        article.Title,
        article.Summary,
        article.Content,
        article.Category,
        Tags = article.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries),
        article.Source,
        article.CreatedAtUtc,
    });
});

app.MapPut("/api/articles/{id:int}", async (int id, UpdatedArticleRequest request, AppDbContext db) =>
{
    var article = await db.KnowledgeArticle.FindAsync(id);

    if (article is null)
    {
        return Results.NotFound(new { message = $"Article with ID {id} was not found"});
    }

    // Validate the input data
    var validationError = ValidateArticleInput(
        request.Title,
        request.Summary,
        request.Content,
        request.Category,
        request.Tags,
        request.Source
    );
    // If validation fails, return a 400 Bad Request with the error message
    if (validationError is not null)
    {
        return Results.BadRequest(new { message = validationError });
    }

    // Update the article properties
    article.Title = request.Title.Trim();
    article.Summary = request.Summary.Trim();
    article.Content = request.Content.Trim();
    article.Category = request.Category.Trim();
    article.Tags = NormaliseTags(request.Tags);
    article.Source = string.IsNullOrWhiteSpace(request.Source) ? null : request.Source.Trim();
    article.UpdatedAtUtc = DateTime.UtcNow;

    await db.SaveChangesAsync(); // Save changes to the database

    return Results.Ok(new
    {
        article.Id,
        article.Title,
        article.Summary,
        article.Content,
        article.Category,
        Tags = article.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries),
        article.Source,
        article.CreatedAtUtc,
        article.UpdatedAtUtc
    });
});

app.MapDelete("/api/articles/{id:int}", async (int id, AppDbContext db) =>
{
   var article = await db.KnowledgeArticle.FindAsync(id);

   if (article is null)
   {
    return Results.NotFound(new { message = $"Article with ID {id} was not found"});
   }

   db.KnowledgeArticle.Remove(article);
   await db.SaveChangesAsync();

   return Results.NoContent(); // Return 204 No Content to indicate successful deletion
});

app.MapGet("/api/categories", async (AppDbContext db) =>
{
   var categories = await db.KnowledgeArticle
    .Select(article => article.Category)
    .Distinct()
    .OrderBy(category => category)
    .ToListAsync(); 

   return Results.Ok(categories);
});

app.MapGet("/api/tags", async (AppDbContext db) =>
{
    var articles = await db.KnowledgeArticle
        .Select(article => article.Tags)
        .ToListAsync(); // Get all tags from articles
    
    var tags = articles 
        .SelectMany(tags => tags.Split(',', StringSplitOptions.RemoveEmptyEntries)) // Split tags into individual tags
        .Select(tag => tag.Trim()) // Trim whitespace from tags
        .Where(tag => !string.IsNullOrWhiteSpace(tag)) // Filter out empty tags
        .Distinct() // Get distinct tags
        .OrderBy(tag => tag) // Order tags alphabetically
        .ToList(); // Convert to list
    
    return Results.Ok(tags);
});

app.Run();

string? ValidateArticleInput(
    string title,
    string summary,
    string content,
    string category,
    string tags,
    string? source)
{
    if (string.IsNullOrWhiteSpace(title) || title.Length > 500)
    {
        return "Title is required and must be less than 500 characters.";
    }

    if (string.IsNullOrWhiteSpace(summary) || summary.Length > 500)
    {
        return "Summary is required and must be less than 500 characters.";
    }

    if (string.IsNullOrWhiteSpace(content))
    {
        return "Content is required.";
    }

    if (string.IsNullOrWhiteSpace(category) || category.Length > 100)
    {
        return "Category is required and must be less than 100 characters.";
    }

    if (string.IsNullOrWhiteSpace(tags) || tags.Length > 500)
    {
        return "Tags are required and must be less than 500 characters.";
    }

    if (!string.IsNullOrWhiteSpace(source) && source.Length > 500)
    {
        return "Source must be less than 500 characters.";
    }

    return null; // No validation errors
}
