using Application.Abstractions;
using Domain.Primitives;
using Mediator;

namespace Application.Carts.EmptyCart;

public sealed class EmptyCartCommandHandler(
    ICartStore cartStore,
    ICurrentUserService currentUser)
    : ICommandHandler<EmptyCartCommand, Result>
{
    public async ValueTask<Result> Handle(EmptyCartCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        await cartStore.DeleteAsync(userId, cancellationToken);
        return Result.Success();
    }
}
