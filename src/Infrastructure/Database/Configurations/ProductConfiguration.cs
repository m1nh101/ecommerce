using Domain.Enums;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new ProductId(value));

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description);

        builder.Property(p => p.Brand)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.CategoryId)
            .HasConversion(id => id.Value, value => new CategoryId(value))
            .IsRequired();

        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.OwnsOne(p => p.BasePrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("base_price_amount")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("base_price_currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();

        builder.HasMany(p => p.Variants)
            .WithOne()
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.Gender);
        builder.HasIndex(p => p.Brand);
        builder.HasIndex(p => p.IsActive);
    }
}
