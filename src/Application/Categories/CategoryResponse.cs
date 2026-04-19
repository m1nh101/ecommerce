namespace Application.Categories;

public sealed record CategoryResponse(Guid Id, string Name, string Slug, string? Description, int DisplayOrder, bool IsActive);
