using Domain.Enums;
using Domain.Primitives;
using Domain.Products;
using Domain.Users;
using Domain.ValueObjects;
using Domain.Orders.Events;

namespace Domain.Orders;

public sealed class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderItem> _items = [];

    private Order() { }

    private Order(OrderId id, UserId userId, Address shippingAddress, DateTime placedAt)
        : base(id)
    {
        UserId = userId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        PlacedAt = placedAt;
        CreatedAt = placedAt;
        UpdatedAt = placedAt;
    }

    public UserId UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public Money SubTotal { get; private set; } = null!;
    public DateTime PlacedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public static Result<Order> Place(
        UserId userId,
        Address shippingAddress,
        IReadOnlyList<(ProductId ProductId, string ProductName, Money UnitPrice, int Quantity)> items)
    {
        if (items.Count == 0)
            return Result.Failure<Order>(OrderErrors.NoItems);

        foreach (var item in items)
            if (item.Quantity <= 0)
                return Result.Failure<Order>(OrderErrors.InvalidItemQuantity);

        var now = DateTime.UtcNow;
        var order = new Order(OrderId.New(), userId, shippingAddress, now);

        foreach (var item in items)
            order._items.Add(OrderItem.Create(order.Id, item.ProductId, item.ProductName, item.UnitPrice, item.Quantity));

        order.SubTotal = order._items
            .Aggregate(Money.Zero(items[0].UnitPrice.Currency), (acc, i) => acc.Add(i.LineTotal));

        order.RaiseDomainEvent(new OrderPlacedDomainEvent(order.Id));
        return order;
    }

    public Result Confirm()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.CannotConfirm);

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new OrderConfirmedDomainEvent(Id));
        return Result.Success();
    }

    public Result Ship()
    {
        if (Status != OrderStatus.Confirmed)
            return Result.Failure(OrderErrors.CannotShip);

        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new OrderShippedDomainEvent(Id));
        return Result.Success();
    }

    public Result Deliver()
    {
        if (Status != OrderStatus.Shipped)
            return Result.Failure(OrderErrors.CannotDeliver);

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
            return Result.Failure(OrderErrors.CannotCancel);

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new OrderCancelledDomainEvent(Id));
        return Result.Success();
    }
}
