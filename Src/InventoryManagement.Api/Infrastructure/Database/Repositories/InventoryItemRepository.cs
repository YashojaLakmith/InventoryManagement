using InventoryManagement.Api.Features.InventoryItems;
using InventoryManagement.Api.Features.InventoryItems.ListItems;
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
            	    inv."InventoryItemId" AS "ItemId",
            	    inv."ItemName" AS "ItemName",
            	    inv."MeasurementUnit" AS "MeasurementUnit",
            	    SUM(b."ReceivedUnits" - b."IssuedUnits") AS "AvailableQuantity",
            	    ROUND(SUM((b."ReceivedUnits" - b."IssuedUnits") * b."CostPerUnit") / COUNT(*), 2) AS "WeightedAverageCostPerUnit"
                FROM "Batches" AS b
                INNER JOIN "InventoryItems" AS inv ON b."InventoryItemId" = inv."InventoryItemId"
                WHERE 
                    b."InventoryItemId" = {itemId} AND
                    b."IssuedUnits" < b."ReceivedUnits"
                GROUP BY inv."InventoryItemId", inv."ItemName", inv."MeasurementUnit"
            """;
    }

    public Task<bool> IsInventoryItemIdInUseAsync(string inventoryItemId, CancellationToken cancellationToken = default)
    {
        return _dbContext.InventoryItems
            .AsNoTracking()
            .AnyAsync(item => item.InventoryItemId == inventoryItemId, cancellationToken);
    }

    public async Task<ListItemsResult> ListItemsAsync(int pageNumber, int countPerPage, string? namePartToSearch, CancellationToken cancellationToken = default)
    {
        int offset = (pageNumber - 1) * countPerPage;

        IQueryable<InventoryItem> query = _dbContext.InventoryItems.AsNoTracking();
        query = string.IsNullOrWhiteSpace(namePartToSearch) || namePartToSearch == string.Empty
            ? query
            : query.Where(item => EF.Functions.Like(item.ItemName, $"%{namePartToSearch}%"));

        Task<List<ListItem>> getItemsTask = query
            .Select(item => new ListItem(item.InventoryItemId, item.ItemName))
            .OrderBy(item => item.ItemId)
            .Skip(offset)
            .Take(countPerPage)
            .ToListAsync(cancellationToken);

        Task<int> getCountTask = query.CountAsync(cancellationToken);

        await Task.WhenAll(getItemsTask, getCountTask);

        return new ListItemsResult(getItemsTask.Result, getItemsTask.Result.Count, pageNumber, getCountTask.Result);
    }
}
