using InventoryManagement.Api.Features.Batches;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Api.Infrastructure.Database;

public class BatchModelConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.HasIndex(prop => prop.BatchNumber)
            .IsUnique(false);

        builder.HasOne(prop => prop.InventoryItem)
            .WithMany()
            .HasForeignKey(@"InventoryItemId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasKey(@"BatchNumber", @"InventoryItemId");

        builder.Property(prop => prop.BatchNumber)
            .IsRequired()
            .HasColumnType(@"varchar(25)");

        builder.Property(prop => prop.CostPerUnit)
            .IsRequired()
            .HasPrecision(12, 2);

        builder.Property(prop => prop.BatchSize)
            .IsRequired()
            .HasPrecision(10, 0);

        builder.Property(prop => prop.AvailableUnits)
            .IsRequired()
            .HasPrecision(10, 0);

        builder.Property<byte[]>(@"RowVersion")
            .IsConcurrencyToken();
    }
}
