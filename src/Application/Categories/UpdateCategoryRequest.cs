namespace Application.Categories;

public sealed record UpdateCategoryRequest(string Name, string Slug, string? Description, int DisplayOrder, bool IsActive);
