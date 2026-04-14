using Domain.Primitives;
using Mediator;

namespace Application.Products.Update;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    string Category,
    bool IsActive) : ICommand<Result>;
