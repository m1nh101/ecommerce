using Application.Abstractions;
using Domain.Carts;
using Redis.OM;
using Redis.OM.Searching;

namespace Infrastructure.Carts;

internal sealed class RedisCartStore(RedisConnectionProvider provider) : ICartStore
{
    private static readonly TimeSpan CartTtl = TimeSpan.FromDays(30);

    private IRedisCollection<CartDocument> Collection =>
        provider.RedisCollection<CartDocument>();

    public async Task<Cart?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var doc = await Collection.FindByIdAsync(userId.ToString());
        if (doc is null)
            return null;

        var cart = Cart.For(userId);
        var items = doc.Items.Select(i =>
            CartItem.Create(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity));
        cart.LoadItems(items);
        return cart;
    }

    public async Task SaveAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        var doc = new CartDocument
        {
            Id = cart.UserId.ToString(),
            UpdatedAt = cart.UpdatedAt,
            Items = cart.Items.Select(i => new CartItemDocument
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };

        await Collection.UpdateAsync(doc, CartTtl);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var doc = await Collection.FindByIdAsync(userId.ToString());
        if (doc is not null)
            await Collection.DeleteAsync(doc);
    }
}
