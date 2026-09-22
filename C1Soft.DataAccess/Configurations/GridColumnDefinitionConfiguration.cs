using C1Soft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C1Soft.DataAccess.Configurations;

public class GridColumnDefinitionConfiguration : IEntityTypeConfiguration<GridColumnDefinition>
{
    public void Configure(EntityTypeBuilder<GridColumnDefinition> builder)
    {
        builder.ToTable("GridColumnDefinitions");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.GridCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.FieldName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.HeaderName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.RenderType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(g => g.Width)
            .HasMaxLength(20);

        builder.Property(g => g.Alignment)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("center");

        // Hızlı sıralama için GridCode ve OrderIndex bileşik index'i
        builder.HasIndex(g => new { g.GridCode, g.OrderIndex });
    }
}
