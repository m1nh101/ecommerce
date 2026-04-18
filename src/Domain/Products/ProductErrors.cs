using Domain.Primitives;

namespace Domain.Products;

public static class ProductErrors
{
    public static readonly Error NotFound = new("Product.NotFound", "The product was not found.");
    public static readonly Error Inactive = new("Product.Inactive", "The product is not available for ordering.");
    public static readonly Error InvalidPrice = new("Product.InvalidPrice", "Price cannot be negative.");
    public static readonly Error InvalidStockQuantity = new("Product.InvalidStockQuantity", "Stock quantity cannot be negative.");
    public static readonly Error InsufficientStock = new("Product.InsufficientStock", "Insufficient stock to fulfill the request.");
    public static readonly Error InvalidQuantity = new("Product.InvalidQuantity", "Quantity must be greater than zero.");
    public static readonly Error DuplicateVariant = new("Product.DuplicateVariant", "A variant with the same color and size already exists for this product.");
    public static readonly Error DuplicateSku = new("Product.DuplicateSku", "A variant with this SKU already exists.");
    public static readonly Error VariantNotFound = new("Product.VariantNotFound", "The specified variant was not found.");
    public static readonly Error ImageNotFound = new("Product.ImageNotFound", "The specified image was not found.");
    public static readonly Error InvalidImageUrl = new("Product.InvalidImageUrl", "Image URL cannot be empty.");
    public static readonly Error InvalidGender = new("Product.InvalidGender", "The specified gender value is not valid.");
}
