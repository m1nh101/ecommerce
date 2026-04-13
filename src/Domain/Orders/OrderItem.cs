using Domain.Primitives;
using Domain.Products;
using Domain.ValueObjects;

namespace Domain.Orders;

public sealed class OrderItem : Entity<OrderItemId>
{
    private OrderItem() { }

    private OrderItem(OrderItemId id, OrderId orderId, ProductId productId, string productName, Money unitPrice, int quantity)
        : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        LineTotal = unitPrice.Multiply(quantity);
    }

    public OrderId OrderId { get; private set; }
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public Money UnitPrice { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Money LineTotal { get; private set; } = null!;

    internal static OrderItem Create(OrderId orderId, ProductId productId, string productName, Money unitPrice, int quantity) =>
        new(OrderItemId.New(), orderId, productId, productName, unitPrice, quantity);
}
