using InventoryManagement.Api.Features.InventoryItems;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Infrastructure.Database.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly ApplicationDbContext _dbContext;
    public InventoryItemRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void CreateNewItem(InventoryItem newItem)
    {
        _dbContext.InventoryItems.Add(newItem);
    }

    public void DeleteItem(InventoryItem existingItem)
    {
        _dbContext.InventoryItems.Remove(existingItem);
    }

    public Task<InventoryItem?> GetInventoryItemByIdAsync(string inventoryItemId, CancellationToken cancellationToken = default)
    {
        return _dbContext.InventoryItems
            .FirstOrDefaultAsync(item => item.InventoryItemId == inventoryItemId, cancellationToken);
    }

    public Task<bool> IsInventoryItemIdInUseAsync(string inventoryItemId, CancellationToken cancellationToken = default)
    {
        return _dbContext.InventoryItems
            .AsNoTracking()
            .AnyAsync(item => item.InventoryItemId == inventoryItemId, cancellationToken);
    }
}
