namespace fiduciary.Contracts;

public record CreatedArticleResponse (
    string Title,
    string Summary,
    string Content,
    string Category,
    string[] Tags,
    string? Source
);