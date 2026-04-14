namespace Application.Products.Detail;

public sealed record ProductDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    string Category,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
