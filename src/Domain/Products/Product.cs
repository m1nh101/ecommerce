using Domain.Primitives;
using Domain.ValueObjects;
using Domain.Products.Events;

namespace Domain.Products;

public sealed class Product : AggregateRoot<ProductId>
{
    private Product() { }

    private Product(ProductId id, string name, string? description, Money price, int stockQuantity, string category)
        : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        Category = category;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Money Price { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public string Category { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Result<Product> Create(string name, string? description, Money price, int stockQuantity, string category)
    {
        if (price.Amount < 0)
            return Result.Failure<Product>(ProductErrors.InvalidPrice);

        if (stockQuantity < 0)
            return Result.Failure<Product>(ProductErrors.InvalidStockQuantity);

        return new Product(ProductId.New(), name, description, price, stockQuantity, category);
    }

    public Result UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount < 0)
            return Result.Failure(ProductErrors.InvalidPrice);

        var oldPrice = Price;
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ProductPriceChangedDomainEvent(Id, oldPrice, newPrice));
        return Result.Success();
    }

    public Result DecrementStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(ProductErrors.InvalidQuantity);

        if (StockQuantity < quantity)
            return Result.Failure(ProductErrors.InsufficientStock);

        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result IncrementStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(ProductErrors.InvalidQuantity);

        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
