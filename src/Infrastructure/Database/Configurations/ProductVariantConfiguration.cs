using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, value => new ProductVariantId(value));

        builder.Property(v => v.ProductId)
            .HasConversion(id => id.Value, value => new ProductId(value))
            .IsRequired();

        builder.Property(v => v.ColorId)
            .HasConversion(id => id.Value, value => new ColorId(value))
            .IsRequired();

        builder.Property(v => v.SizeId)
            .HasConversion(id => id.Value, value => new SizeId(value))
            .IsRequired();

        builder.Property(v => v.Sku)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(v => v.PriceOverride, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("price_override_amount")
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .HasColumnName("price_override_currency")
                .HasMaxLength(3);
        });

        builder.Property(v => v.StockQuantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(v => v.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(v => v.CreatedAt).IsRequired();
        builder.Property(v => v.UpdatedAt).IsRequired();

        builder.HasIndex(v => v.Sku).IsUnique();
        builder.HasIndex(v => new { v.ProductId, v.ColorId, v.SizeId }).IsUnique();
    }
}
