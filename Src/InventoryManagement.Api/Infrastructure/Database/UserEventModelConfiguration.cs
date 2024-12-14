using InventoryManagement.Api.Features.UserEvents;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Api.Infrastructure.Database;

public class UserEventModelConfiguration : IEntityTypeConfiguration<UserEvent>
{
    public void Configure(EntityTypeBuilder<UserEvent> builder)
    {
        builder.HasNoKey();
    }
}
