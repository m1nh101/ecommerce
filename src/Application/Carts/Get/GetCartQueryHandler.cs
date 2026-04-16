using Application.Abstractions;
using Domain.Primitives;
using Mediator;

namespace Application.Carts.Get;

public sealed class GetCartQueryHandler(
    ICartStore cartStore,
    ICurrentUserService currentUser)
    : IQueryHandler<GetCartQuery, Result<CartResponse>>
{
    public async ValueTask<Result<CartResponse>> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        var cart = await cartStore.GetAsync(userId, cancellationToken);

        if (cart is null)
        {
            return new CartResponse(
                UserId: userId,
                Items: [],
                TotalPrice: 0,
                UpdatedAt: DateTime.UtcNow);
        }

        var items = cart.Items
            .Select(i => new CartItemResponse(
                ProductId: i.ProductId,
                ProductName: i.ProductName,
                UnitPrice: i.UnitPrice,
                Quantity: i.Quantity,
                SubTotal: i.UnitPrice * i.Quantity))
            .ToList();

        return new CartResponse(
            UserId: cart.UserId,
            Items: items,
            TotalPrice: items.Sum(i => i.SubTotal),
            UpdatedAt: cart.UpdatedAt);
    }
}
