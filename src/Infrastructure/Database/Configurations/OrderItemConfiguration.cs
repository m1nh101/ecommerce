using Domain.Orders;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, value => new OrderItemId(value));

        builder.Property(i => i.OrderId)
            .HasConversion(id => id.Value, value => new OrderId(value))
            .IsRequired();

        builder.Property(i => i.ProductId)
            .HasConversion(id => id.Value, value => new ProductId(value))
            .IsRequired();

        builder.Property(i => i.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("unit_price_amount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("unit_price_currency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(i => i.LineTotal, money =>
        {
            money.Property(m => m.Amount).HasColumnName("line_total_amount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("line_total_currency").HasMaxLength(3).IsRequired();
        });

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.HasIndex(i => i.OrderId);
        builder.HasIndex(i => i.ProductId);
    }
}
