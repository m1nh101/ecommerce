using Domain.Primitives;

namespace Domain.Products;

public sealed class Color : AggregateRoot<ColorId>
{
    private Color() { }

    private Color(ColorId id, string name, string hexCode) : base(id)
    {
        Name = name;
        HexCode = hexCode;
    }

    public string Name { get; private set; } = null!;
    public string HexCode { get; private set; } = null!;

    public static Result<Color> Create(string name, string hexCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Color>(ColorErrors.InvalidName);

        if (!IsValidHexCode(hexCode))
            return Result.Failure<Color>(ColorErrors.InvalidHexCode);

        return new Color(ColorId.New(), name, hexCode);
    }

    public Result UpdateHexCode(string hexCode)
    {
        if (!IsValidHexCode(hexCode))
            return Result.Failure(ColorErrors.InvalidHexCode);

        HexCode = hexCode;
        return Result.Success();
    }

    private static bool IsValidHexCode(string hexCode) =>
        hexCode is { Length: 7 } && hexCode[0] == '#' &&
        hexCode[1..].All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
}
