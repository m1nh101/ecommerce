using Domain.Primitives;

namespace Domain.Products;

public static class ProductErrors
{
    public static readonly Error NotFound = new("Product.NotFound", "The product was not found.");
    public static readonly Error InsufficientStock = new("Product.InsufficientStock", "Insufficient stock to fulfill the request.");
    public static readonly Error Inactive = new("Product.Inactive", "The product is not available for ordering.");
    public static readonly Error InvalidPrice = new("Product.InvalidPrice", "Price cannot be negative.");
    public static readonly Error InvalidStockQuantity = new("Product.InvalidStockQuantity", "Stock quantity cannot be negative.");
    public static readonly Error InvalidQuantity = new("Product.InvalidQuantity", "Quantity must be greater than zero.");
}
