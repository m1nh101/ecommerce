using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => new UserAddressId(value));

        builder.Property(a => a.UserId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(a => a.Label)
            .HasMaxLength(100);

        builder.Property(a => a.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.OwnsOne(a => a.Address, address =>
        {
            address.Property(x => x.Street).HasColumnName("street").HasMaxLength(200).IsRequired();
            address.Property(x => x.City).HasColumnName("city").HasMaxLength(100).IsRequired();
            address.Property(x => x.State).HasColumnName("state").HasMaxLength(100).IsRequired();
            address.Property(x => x.Country).HasColumnName("country").HasMaxLength(100).IsRequired();
            address.Property(x => x.PostalCode).HasColumnName("postal_code").HasMaxLength(20).IsRequired();
        });

        builder.HasIndex(a => a.UserId);
    }
}
