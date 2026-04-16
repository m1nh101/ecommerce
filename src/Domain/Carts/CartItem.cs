namespace Domain.Carts;

public sealed class CartItem
{
    private CartItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public static CartItem Create(Guid productId, string productName, decimal unitPrice, int quantity)
        => new(productId, productName, unitPrice, quantity);

    internal void SetQuantity(int quantity) => Quantity = quantity;

    internal void IncreaseQuantity(int amount) => Quantity += amount;
}
