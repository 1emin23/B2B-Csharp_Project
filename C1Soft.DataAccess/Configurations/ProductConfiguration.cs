using C1Soft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C1Soft.DataAccess.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        // Benzersiz ürün kodu (Unique Index)
        builder.Property(p => p.ProductCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.ProductCode)
            .IsUnique();

        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Brand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ManufacturerCode)
            .HasMaxLength(100);

        builder.Property(p => p.CustomCode1)
            .HasMaxLength(100);

        builder.Property(p => p.CustomCode2)
            .HasMaxLength(100);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        // SQL Server decimal(18,2) standardı
        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.StockQuantity)
            .IsRequired();

        builder.Property(p => p.CriticalStockThreshold)
            .IsRequired()
            .HasDefaultValue(10);

        // StockStatus hesaplanan bir özellik olduğu için DB'ye kolon olarak eklenmez
        builder.Ignore(p => p.StockStatus);
    }
}
