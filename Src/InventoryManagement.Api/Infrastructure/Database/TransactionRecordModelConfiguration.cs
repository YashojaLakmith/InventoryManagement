using InventoryManagement.Api.Features.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Api.Infrastructure.Database;

public class TransactionRecordModelConfiguration : IEntityTypeConfiguration<TransactionRecord>
{
    public void Configure(EntityTypeBuilder<TransactionRecord> builder)
    {
        builder.HasKey(prop => prop.RecordId);

        builder.HasOne(prop => prop.InventoryItem)
            .WithOne()
            .HasForeignKey<TransactionRecord>()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(prop => prop.BatchOfItem)
            .WithOne()
            .HasForeignKey<TransactionRecord>()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(prop => prop.RecordId)
            .IsRequired()
            .UseIdentityColumn();

        builder.Property(prop => prop.TransactionUnitCount)
            .IsRequired()
            .HasPrecision(10, 0);

        builder.Property(prop => prop.Timestamp)
            .IsRequired()
            .HasColumnType(@"timestamp with time zone");
    }
}
