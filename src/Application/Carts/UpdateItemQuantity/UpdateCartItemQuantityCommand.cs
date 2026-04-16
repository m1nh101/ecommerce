using Domain.Primitives;
using Mediator;

namespace Application.Carts.UpdateItemQuantity;

public record UpdateCartItemQuantityCommand(
    Guid ProductId,
    int Quantity) : ICommand<Result>;
