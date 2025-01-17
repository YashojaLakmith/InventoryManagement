using InventoryManagement.Api.Features.InventoryItems;
using InventoryManagement.Api.Features.InventoryItems.ViewItem;

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

    public Task<ItemDetails?> GetItemDetailsByIdAsync(string itemId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Database
            .SqlQuery<ItemDetails>(GetQueryString(itemId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static FormattableString GetQueryString(string itemId)
    {
        return $"""
                SELECT 
            	    i."InventoryItemId" AS "ItemId",
            	    i."ItemName" AS "ItemName",
            	    i."MeasurementUnit" AS "MeasurementUnit",
            	    SUM(b."ReceivedUnits" - b."IssuedUnits") AS "AvailableQuantity",
            	    ROUND(SUM((b."ReceivedUnits" - b."IssuedUnits") * b."CostPerUnit") / COUNT(*), 2) AS "WeightedAverageCostOfUnit"
                FROM "Batches" AS b
                INNER JOIN "InventoryItems" AS i ON b."InventoryItemId" = i."InventoryItemId"
                WHERE 
                    b."InventoryItemId" = {itemId} AND
                    b."IssuedUnits" < b."ReceivedUnits"
                GROUP BY i."InventoryItemId", i."ItemName", i."MeasurementUnit"
            """;
    }

    public Task<bool> IsInventoryItemIdInUseAsync(string inventoryItemId, CancellationToken cancellationToken = default)
    {
        return _dbContext.InventoryItems
            .AsNoTracking()
            .AnyAsync(item => item.InventoryItemId == inventoryItemId, cancellationToken);
    }
}
