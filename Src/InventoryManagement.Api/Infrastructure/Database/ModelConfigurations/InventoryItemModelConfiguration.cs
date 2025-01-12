using InventoryManagement.Api.Features.InventoryItems;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Api.Infrastructure.Database.ModelConfigurations;

public class InventoryItemModelConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(prop => prop.InventoryItemId);

        builder.Property(prop => prop.InventoryItemId)
            .HasColumnType(@"varchar(25)");

        builder.Property(prop => prop.MeasurementUnit)
            .IsRequired()
            .HasColumnType(@"varchar(10)");

        builder.Property(prop => prop.ItemName)
            .IsRequired()
            .HasColumnType(@"varchar(100)");
    }
}
