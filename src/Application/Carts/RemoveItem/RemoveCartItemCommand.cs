using Domain.Primitives;
using Mediator;

namespace Application.Carts.RemoveItem;

public record RemoveCartItemCommand(Guid ProductId) : ICommand<Result>;
