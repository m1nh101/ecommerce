namespace Domain.Products;

public readonly record struct ProductVariantId(Guid Value)
{
    public static ProductVariantId New() => new(Guid.NewGuid());
}
