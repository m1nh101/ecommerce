using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Database.Configurations;

internal sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_images");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, value => new ProductImageId(value));

        builder.Property(i => i.ProductId)
            .HasConversion(id => id.Value, value => new ProductId(value))
            .IsRequired();

        builder.Property(i => i.VariantId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new ProductVariantId(value.Value) : null);

        builder.Property(i => i.Url)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(i => i.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(i => i.SortOrder)
            .IsRequired();

        builder.HasIndex(i => i.ProductId);
    }
}
