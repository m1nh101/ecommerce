using Application.Abstractions;
using Domain.Carts;
using Domain.Primitives;
using Mediator;

namespace Application.Carts.RemoveItem;

public sealed class RemoveCartItemCommandHandler(
    ICartStore cartStore,
    ICurrentUserService currentUser)
    : ICommandHandler<RemoveCartItemCommand, Result>
{
    public async ValueTask<Result> Handle(RemoveCartItemCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        var cart = await cartStore.GetAsync(userId, cancellationToken);

        if (cart is null)
            return Result.Failure(CartErrors.ItemNotFound);

        var result = cart.RemoveItem(command.ProductId);
        if (result.IsFailure)
            return result;

        await cartStore.SaveAsync(cart, cancellationToken);
        return Result.Success();
    }
}
