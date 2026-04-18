using Domain.Primitives;

namespace Domain.Products;

public static class SizeErrors
{
    public static readonly Error NotFound = new("Size.NotFound", "The size was not found.");
    public static readonly Error DuplicateNameAndType = new("Size.Duplicate", "A size with this name and type already exists.");
    public static readonly Error InvalidName = new("Size.InvalidName", "Size name cannot be empty.");
}
