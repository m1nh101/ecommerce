using Domain.Primitives;

namespace Domain.Products;

public static class ColorErrors
{
    public static readonly Error NotFound = new("Color.NotFound", "The color was not found.");
    public static readonly Error InvalidHexCode = new("Color.InvalidHexCode", "Hex code must be a 7-character string starting with '#' followed by 6 hex digits.");
    public static readonly Error DuplicateName = new("Color.DuplicateName", "A color with this name already exists.");
    public static readonly Error InvalidName = new("Color.InvalidName", "Color name cannot be empty.");
}
