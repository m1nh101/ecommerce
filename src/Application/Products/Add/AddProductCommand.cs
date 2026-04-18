using Domain.Primitives;
using Mediator;

namespace Application.Products.Add;

public record AddProductCommand(
    string Name,
    string? Description,
    string Brand,
    Guid CategoryId,
    string Gender,
    decimal BasePrice,
    string Currency) : ICommand<Result<Guid>>;
