using Domain.Primitives;

namespace Domain.Carts;

public sealed class Cart
{
    private readonly List<CartItem> _items = [];

    private Cart(Guid userId)
    {
        UserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid UserId { get; private set; }
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public DateTime UpdatedAt { get; private set; }

    public static Cart For(Guid userId) => new(userId);

    public Result AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(CartErrors.InvalidQuantity);

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
            existing.IncreaseQuantity(quantity);
        else
            _items.Add(CartItem.Create(productId, productName, unitPrice, quantity));

        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdateItemQuantity(Guid productId, int newQuantity)
    {
        if (newQuantity <= 0)
            return Result.Failure(CartErrors.InvalidQuantity);

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        item.SetQuantity(newQuantity);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        _items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void Empty()
    {
        _items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    // Used by infrastructure to reconstruct from storage
    public void LoadItems(IEnumerable<CartItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
    }
}
