using Application.Abstractions;
using Domain.Carts;
using Domain.Primitives;
using Mediator;

namespace Application.Carts.UpdateItemQuantity;

public sealed class UpdateCartItemQuantityCommandHandler(
    ICartStore cartStore,
    ICurrentUserService currentUser)
    : ICommandHandler<UpdateCartItemQuantityCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateCartItemQuantityCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        var cart = await cartStore.GetAsync(userId, cancellationToken);

        if (cart is null)
            return Result.Failure(CartErrors.ItemNotFound);

        var result = cart.UpdateItemQuantity(command.ProductId, command.Quantity);
        if (result.IsFailure)
            return result;

        await cartStore.SaveAsync(cart, cancellationToken);
        return Result.Success();
    }
}
