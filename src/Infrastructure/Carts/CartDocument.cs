using Redis.OM.Modeling;

namespace Infrastructure.Carts;

[Document(StorageType = StorageType.Json, Prefixes = ["cart"], IndexName = "cart-idx")]
public class CartDocument
{
    [RedisIdField]
    [Indexed]
    public string Id { get; set; } = null!; // userId.ToString()

    public List<CartItemDocument> Items { get; set; } = [];

    public DateTime UpdatedAt { get; set; }
}
