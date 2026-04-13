using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new ProductId(value));

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description);

        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(m => m.Amount).HasColumnName("price").HasPrecision(18, 2).IsRequired();
            price.Property(m => m.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired().HasDefaultValue("USD");
        });

        builder.Property(p => p.StockQuantity)
            .IsRequired();

        builder.Property(p => p.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.IsActive);
    }
}
