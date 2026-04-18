namespace Domain.Products;

public readonly record struct ProductImageId(Guid Value)
{
    public static ProductImageId New() => new(Guid.NewGuid());
}
