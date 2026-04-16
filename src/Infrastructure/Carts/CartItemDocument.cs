using Redis.OM.Modeling;

namespace Infrastructure.Carts;

public class CartItemDocument
{
    [Indexed]
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
}
