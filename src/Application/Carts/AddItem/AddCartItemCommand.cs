using Domain.Primitives;
using Mediator;

namespace Application.Carts.AddItem;

public record AddCartItemCommand(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity) : ICommand<Result>;
