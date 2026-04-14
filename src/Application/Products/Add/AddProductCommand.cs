using Domain.Primitives;
using Mediator;

namespace Application.Products.Add;

public record AddProductCommand(
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    string Category) : ICommand<Result<Guid>>;
