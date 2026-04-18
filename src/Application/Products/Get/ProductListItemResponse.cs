namespace Application.Products.Get;

public sealed record ProductListItemResponse(
    Guid Id,
    string Name,
    string? Description,
    string Brand,
    Guid CategoryId,
    string Gender,
    decimal BasePriceAmount,
    string BasePriceCurrency,
    bool IsActive,
    DateTime CreatedAt);
