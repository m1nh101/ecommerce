using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Products;

public sealed class ProductVariant : Entity<ProductVariantId>
{
    private ProductVariant() { }

    internal ProductVariant(
        ProductVariantId id,
        ProductId productId,
        ColorId colorId,
        SizeId sizeId,
        string sku,
        Money? priceOverride,
        int stockQuantity)
        : base(id)
    {
        ProductId = productId;
        ColorId = colorId;
        SizeId = sizeId;
        Sku = sku;
        PriceOverride = priceOverride;
        StockQuantity = stockQuantity;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public ProductId ProductId { get; private set; }
    public ColorId ColorId { get; private set; }
    public SizeId SizeId { get; private set; }
    public string Sku { get; private set; } = null!;
    public Money? PriceOverride { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    internal Result SetPriceOverride(Money? priceOverride)
    {
        if (priceOverride is not null && priceOverride.Amount < 0)
            return Result.Failure(ProductErrors.InvalidPrice);

        PriceOverride = priceOverride;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    internal Result AdjustStock(int delta)
    {
        var newStock = StockQuantity + delta;
        if (newStock < 0)
            return Result.Failure(ProductErrors.InsufficientStock);

        StockQuantity = newStock;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    internal void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
