﻿using InventoryManagement.Api.Features.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Api.Infrastructure.Database.ModelConfigurations;

public class UserModelConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(prop => prop.Id)
            .IsRequired()
            .UseIdentityAlwaysColumn();

        builder.Property(prop => prop.Email)
            .IsRequired();

        builder.HasIndex(prop => prop.Email)
            .IsUnique();

        builder.Property(prop => prop.UserName)
            .IsRequired();
    }
}
