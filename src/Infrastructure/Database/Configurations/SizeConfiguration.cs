using Domain.Enums;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class SizeConfiguration : IEntityTypeConfiguration<Size>
{
    public void Configure(EntityTypeBuilder<Size> builder)
    {
        builder.ToTable("sizes");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => new SizeId(value));

        builder.Property(s => s.Name)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.SizeType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.SortOrder)
            .IsRequired();

        builder.HasIndex(s => new { s.Name, s.SizeType }).IsUnique();
        builder.HasIndex(s => new { s.SizeType, s.SortOrder });
    }
}
