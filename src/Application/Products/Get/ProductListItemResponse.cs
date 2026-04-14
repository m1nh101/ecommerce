namespace Application.Products.Get;

public sealed record ProductListItemResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    string Category,
    bool IsActive,
    DateTime CreatedAt);
