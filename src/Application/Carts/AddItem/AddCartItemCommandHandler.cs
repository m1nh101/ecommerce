using Application.Abstractions;
using Domain.Carts;
using Domain.Primitives;
using Mediator;

namespace Application.Carts.AddItem;

public sealed class AddCartItemCommandHandler(
    ICartStore cartStore,
    ICurrentUserService currentUser)
    : ICommandHandler<AddCartItemCommand, Result>
{
    public async ValueTask<Result> Handle(AddCartItemCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        var cart = await cartStore.GetAsync(userId, cancellationToken) ?? Cart.For(userId);

        var result = cart.AddItem(command.ProductId, command.ProductName, command.UnitPrice, command.Quantity);
        if (result.IsFailure)
            return result;

        await cartStore.SaveAsync(cart, cancellationToken);
        return Result.Success();
    }
}
