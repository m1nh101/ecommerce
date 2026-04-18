using Domain.Orders;
using Domain.Payments;
using Domain.Products;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Color> Colors { get; }
    DbSet<Size> Sizes { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<Order> Orders { get; }
    DbSet<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
