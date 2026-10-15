using ManagementUtility.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagementUtility.Infrastructure.Persistence.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(asset => asset.Id);

        builder.Property(asset => asset.InventoryNumber)
            .IsRequired()
            .HasMaxLength(50)
            .UseCollation("NOCASE");

        builder.HasIndex(asset => asset.InventoryNumber)
            .IsUnique();

        builder.Property(asset => asset.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(asset => asset.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(asset => asset.Location)
            .HasMaxLength(150);

        builder.Property(asset => asset.ResponsiblePerson)
            .HasMaxLength(150);

        builder.Property(asset => asset.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(asset => asset.PurchasePrice)
            .HasPrecision(12, 2);

        builder.Property(asset => asset.Notes)
            .HasMaxLength(1000);
    }
}