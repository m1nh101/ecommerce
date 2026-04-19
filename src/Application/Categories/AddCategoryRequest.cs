namespace Application.Categories;

public sealed record AddCategoryRequest(string Name, string Slug, string? Description, int DisplayOrder);
