namespace Application.Carts.Get;

public record CartResponse(
    Guid UserId,
    IReadOnlyList<CartItemResponse> Items,
    decimal TotalPrice,
    DateTime UpdatedAt);

public record CartItemResponse(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal SubTotal);
