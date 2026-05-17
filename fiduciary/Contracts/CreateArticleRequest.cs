namespace fiduciary.Contracts;

public record CreateArticleRequest (
    string Title,
    string Summary,
    string Content,
    string Category,
    string Tags,
    string? Source
);