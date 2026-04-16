using Domain.Primitives;
using Mediator;

namespace Application.Carts.EmptyCart;

public record EmptyCartCommand : ICommand<Result>;
