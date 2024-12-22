﻿using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.InventoryItems;
using InventoryManagement.Api.Features.Transactions;
using InventoryManagement.Api.Features.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Infrastructure.Database;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<InventoryItem> InventoryItems { get; private set; }
    public DbSet<Batch> Batches { get; private set; }
    public DbSet<TransactionRecord> TransactionRecords { get; private set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(@"Host=127.0.0.1;Database=InventoryManagement;Username=postgres;Password=postgres;");
}