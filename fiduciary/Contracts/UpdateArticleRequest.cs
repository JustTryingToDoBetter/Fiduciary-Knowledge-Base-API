namespace fiduciary.Contracts;

public record UpdateArticleRequest(
    string Title, 
    string Summary,
    string Content,
    string Category,
    string[] Tags,
    string? Source
);