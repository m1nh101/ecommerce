using Domain.Primitives;
using Mediator;

namespace Application.Carts.Get;

public record GetCartQuery : IQuery<Result<CartResponse>>;
