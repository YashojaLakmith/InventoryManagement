using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Api.Infrastructure.Database.ModelConfigurations;

public class RoleModelConfiguration : IEntityTypeConfiguration<IdentityRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<int>> builder)
    {
        builder.Property(prop => prop.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(prop => prop.Name)
            .IsRequired();
    }
}
