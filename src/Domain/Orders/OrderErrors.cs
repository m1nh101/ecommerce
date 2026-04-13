using Domain.Primitives;

namespace Domain.Orders;

public static class OrderErrors
{
    public static readonly Error NotFound = new("Order.NotFound", "The order was not found.");
    public static readonly Error NoItems = new("Order.NoItems", "An order must contain at least one item.");
    public static readonly Error CannotCancel = new("Order.CannotCancel", "Only pending or confirmed orders can be cancelled.");
    public static readonly Error CannotConfirm = new("Order.CannotConfirm", "Only pending orders can be confirmed.");
    public static readonly Error CannotShip = new("Order.CannotShip", "Only confirmed orders can be shipped.");
    public static readonly Error CannotDeliver = new("Order.CannotDeliver", "Only shipped orders can be delivered.");
    public static readonly Error InvalidItemQuantity = new("Order.InvalidItemQuantity", "Item quantity must be greater than zero.");
}
