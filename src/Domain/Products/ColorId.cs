namespace Domain.Products;

public readonly record struct ColorId(Guid Value)
{
    public static ColorId New() => new(Guid.NewGuid());
}
