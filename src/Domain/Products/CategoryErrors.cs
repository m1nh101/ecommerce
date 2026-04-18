using Domain.Primitives;

namespace Domain.Products;

public static class CategoryErrors
{
    public static readonly Error NotFound = new("Category.NotFound", "The category was not found.");
    public static readonly Error DuplicateSlug = new("Category.DuplicateSlug", "A category with this slug already exists.");
    public static readonly Error InvalidName = new("Category.InvalidName", "Category name cannot be empty.");
    public static readonly Error InvalidSlug = new("Category.InvalidSlug", "Category slug cannot be empty.");
}
