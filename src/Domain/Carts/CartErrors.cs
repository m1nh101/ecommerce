using Domain.Primitives;

namespace Domain.Carts;

public static class CartErrors
{
    public static readonly Error ItemNotFound = Error.NotFound(
        "Cart.ItemNotFound",
        "The specified item was not found in the cart.");

    public static readonly Error InvalidQuantity = Error.Validation(
        "Cart.InvalidQuantity",
        "Quantity must be greater than zero.");
}
