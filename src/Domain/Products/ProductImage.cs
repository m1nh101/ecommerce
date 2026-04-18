using Domain.Primitives;

namespace Domain.Products;

public sealed class ProductImage : Entity<ProductImageId>
{
    private ProductImage() { }

    internal ProductImage(
        ProductImageId id,
        ProductId productId,
        ProductVariantId? variantId,
        string url,
        bool isPrimary,
        int sortOrder)
        : base(id)
    {
        ProductId = productId;
        VariantId = variantId;
        Url = url;
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
    }

    public ProductId ProductId { get; private set; }
    public ProductVariantId? VariantId { get; private set; }
    public string Url { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public int SortOrder { get; private set; }

    internal void SetAsPrimary() => IsPrimary = true;
    internal void UnsetPrimary() => IsPrimary = false;
    internal void UpdateSortOrder(int sortOrder) => SortOrder = sortOrder;
}
