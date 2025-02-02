using InventoryManagement.Api.Features;
using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.InventoryItems;
using InventoryManagement.Api.Features.Transactions;
using InventoryManagement.Api.Features.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Infrastructure.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole<int>, int>(options), IUnitOfWork
{
    public DbSet<InventoryItem> InventoryItems { get; private set; }
    public DbSet<Batch> Batches { get; private set; }
    public DbSet<TransactionRecord> TransactionRecords { get; private set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public void ClearChanges()
    {
        ChangeTracker.Clear();
    }
}
