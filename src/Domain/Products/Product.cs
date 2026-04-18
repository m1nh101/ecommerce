using Domain.Enums;
using Domain.Primitives;
using Domain.Products.Events;
using Domain.ValueObjects;

namespace Domain.Products;

public sealed class Product : AggregateRoot<ProductId>
{
    private readonly List<ProductVariant> _variants = [];
    private readonly List<ProductImage> _images = [];

    private Product() { }

    private Product(
        ProductId id,
        string name,
        string? description,
        string brand,
        CategoryId categoryId,
        Gender gender,
        Money basePrice)
        : base(id)
    {
        Name = name;
        Description = description;
        Brand = brand;
        CategoryId = categoryId;
        Gender = gender;
        BasePrice = basePrice;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string Brand { get; private set; } = null!;
    public CategoryId CategoryId { get; private set; }
    public Gender Gender { get; private set; }
    public Money BasePrice { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<ProductVariant> Variants => _variants.AsReadOnly();
    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();

    public static Result<Product> Create(
        string name,
        string? description,
        string brand,
        CategoryId categoryId,
        Gender gender,
        Money basePrice)
    {
        if (basePrice.Amount < 0)
            return Result.Failure<Product>(ProductErrors.InvalidPrice);

        return new Product(ProductId.New(), name, description, brand, categoryId, gender, basePrice);
    }

    public void UpdateDetails(string name, string? description, string brand, CategoryId categoryId, Gender gender)
    {
        Name = name;
        Description = description;
        Brand = brand;
        CategoryId = categoryId;
        Gender = gender;
        UpdatedAt = DateTime.UtcNow;
    }

    public Result UpdateBasePrice(Money newPrice)
    {
        if (newPrice.Amount < 0)
            return Result.Failure(ProductErrors.InvalidPrice);

        var oldPrice = BasePrice;
        BasePrice = newPrice;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ProductPriceChangedDomainEvent(Id, oldPrice, newPrice));
        return Result.Success();
    }

    public Result<ProductVariant> AddVariant(
        ColorId colorId,
        SizeId sizeId,
        string sku,
        Money? priceOverride,
        int initialStock)
    {
        if (_variants.Any(v => v.ColorId == colorId && v.SizeId == sizeId))
            return Result.Failure<ProductVariant>(ProductErrors.DuplicateVariant);

        if (_variants.Any(v => v.Sku == sku))
            return Result.Failure<ProductVariant>(ProductErrors.DuplicateSku);

        if (initialStock < 0)
            return Result.Failure<ProductVariant>(ProductErrors.InvalidStockQuantity);

        if (priceOverride is not null && priceOverride.Amount < 0)
            return Result.Failure<ProductVariant>(ProductErrors.InvalidPrice);

        var variant = new ProductVariant(
            ProductVariantId.New(), Id, colorId, sizeId, sku, priceOverride, initialStock);

        _variants.Add(variant);
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ProductVariantAddedDomainEvent(Id, variant.Id, sku));
        return variant;
    }

    public Result AdjustVariantStock(ProductVariantId variantId, int delta)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null)
            return Result.Failure(ProductErrors.VariantNotFound);

        return variant.AdjustStock(delta);
    }

    public Result SetVariantPriceOverride(ProductVariantId variantId, Money? priceOverride)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null)
            return Result.Failure(ProductErrors.VariantNotFound);

        return variant.SetPriceOverride(priceOverride);
    }

    public Result ActivateVariant(ProductVariantId variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null)
            return Result.Failure(ProductErrors.VariantNotFound);

        variant.Activate();
        return Result.Success();
    }

    public Result DeactivateVariant(ProductVariantId variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null)
            return Result.Failure(ProductErrors.VariantNotFound);

        variant.Deactivate();
        return Result.Success();
    }

    public Result<ProductImage> AddImage(string url, ProductVariantId? variantId, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Result.Failure<ProductImage>(ProductErrors.InvalidImageUrl);

        var image = new ProductImage(ProductImageId.New(), Id, variantId, url, false, sortOrder);
        _images.Add(image);
        UpdatedAt = DateTime.UtcNow;
        return image;
    }

    public Result SetPrimaryImage(ProductImageId imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
            return Result.Failure(ProductErrors.ImageNotFound);

        // Clear existing primary in the same scope (same variantId or product-level)
        foreach (var existing in _images.Where(i => i.VariantId == image.VariantId && i.IsPrimary))
            existing.UnsetPrimary();

        image.SetAsPrimary();
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveImage(ProductImageId imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
            return Result.Failure(ProductErrors.ImageNotFound);

        _images.Remove(image);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
