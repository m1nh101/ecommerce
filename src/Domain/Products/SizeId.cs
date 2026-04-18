namespace Domain.Products;

public readonly record struct SizeId(Guid Value)
{
    public static SizeId New() => new(Guid.NewGuid());
}
