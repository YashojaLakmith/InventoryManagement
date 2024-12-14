using InventoryManagement.Api.Features.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Api.Infrastructure.Database;

public class UserModelConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(prop => prop.Id)
            .IsRequired()
            .UseSerialColumn();

        builder.HasIndex(prop => prop.Email)
            .IsUnique();
    }
}
