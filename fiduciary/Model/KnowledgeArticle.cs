namespace fiduciary.Models;

public class KnowledgeArticle
{
    public int Id {get; set;}

    public required string Title {get;set;}

    public required string Summary {get;set;}

    public required string Content {get;set;}

    public required string Category {get;set;}

    public required string Tags {get;set;}

    public string? Source {get;set;}

    public DateTime CreatedAtUtc = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc {get;set;}
}