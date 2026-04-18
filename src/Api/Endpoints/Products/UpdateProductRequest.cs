namespace Api.Endpoints.Products;

public record UpdateProductRequest(
    string Name,
    string? Description,
    string Brand,
    Guid CategoryId,
    string Gender,
    decimal BasePrice,
    string Currency,
    bool IsActive);
