using InventoryManagement.Api.Features.InventoryItems;

namespace InventoryManagement.Api.Infrastructure.Database.Repositories;

public static class RepositoryExtensions
{
    public static void AddRepositoryImplementations(this IServiceCollection services)
    {
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
    }
}
