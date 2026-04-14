namespace Api.Endpoints.Products;

public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    string Category,
    bool IsActive);
