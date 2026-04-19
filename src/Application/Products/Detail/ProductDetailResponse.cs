namespace Application.Products.Detail;

public sealed record ProductDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    string Brand,
    Guid CategoryId,
    string Gender,
    decimal BasePriceAmount,
    string BasePriceCurrency,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<ProductVariantResponse> Variants,
    IReadOnlyList<ProductImageResponse> Images);

public sealed record ProductVariantResponse(
    Guid Id,
    Guid ColorId,
    Guid SizeId,
    string Sku,
    decimal? PriceOverrideAmount,
    string? PriceOverrideCurrency,
    int StockQuantity,
    string StockStatus,
    bool IsActive);

public sealed record ProductImageResponse(
    Guid Id,
    Guid? VariantId,
    string Url,
    bool IsPrimary,
    int SortOrder);
