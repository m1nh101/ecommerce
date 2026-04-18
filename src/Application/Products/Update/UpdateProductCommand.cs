using Domain.Primitives;
using Mediator;

namespace Application.Products.Update;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? Description,
    string Brand,
    Guid CategoryId,
    string Gender,
    decimal BasePrice,
    string Currency,
    bool IsActive) : ICommand<Result>;
