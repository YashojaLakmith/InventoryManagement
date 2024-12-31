using InventoryManagement.Api.Features;
using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.InventoryItems;
using InventoryManagement.Api.Features.Transactions;
using InventoryManagement.Api.Features.Users;

namespace InventoryManagement.Api.Infrastructure.Database.Repositories;

public static class RepositoryExtensions
{
    public static void AddRepositoryImplementations(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, ApplicationDbContext>();
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBatchRepository, BatchRepository>();
    }
}
