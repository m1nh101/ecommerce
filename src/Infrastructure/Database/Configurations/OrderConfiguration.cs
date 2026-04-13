using Domain.Enums;
using Domain.Orders;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(id => id.Value, value => new OrderId(value));

        builder.Property(o => o.UserId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue(OrderStatus.Pending);

        builder.OwnsOne(o => o.SubTotal, money =>
        {
            money.Property(m => m.Amount).HasColumnName("subtotal_amount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("subtotal_currency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("shipping_street").HasMaxLength(200).IsRequired();
            address.Property(a => a.City).HasColumnName("shipping_city").HasMaxLength(100).IsRequired();
            address.Property(a => a.State).HasColumnName("shipping_state").HasMaxLength(100).IsRequired();
            address.Property(a => a.Country).HasColumnName("shipping_country").HasMaxLength(100).IsRequired();
            address.Property(a => a.PostalCode).HasColumnName("shipping_postal_code").HasMaxLength(20).IsRequired();
        });

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.PlacedAt);
    }
}
