using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.Batches.ListBatchesByBatchNumber;
using InventoryManagement.Api.Features.InventoryItems;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace InventoryManagement.Api.Infrastructure.Database.Repositories;

public class BatchRepository : IBatchRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly HybridCache _hybridCache;
    private static readonly HybridCacheEntryOptions BatchCountEntryOptions = new() { Expiration = TimeSpan.FromHours(2) };

    public BatchRepository(ApplicationDbContext dbContext, HybridCache hybridCache)
    {
        _dbContext = dbContext;
        _hybridCache = hybridCache;
    }

    public void AddBatchLines(IReadOnlyCollection<Batch> batchLines)
    {
        _dbContext.Batches.AddRange(batchLines);
    }

    public Task<bool> DoesBatchExistAsync(string batchId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Batches
            .AsNoTracking()
            .AnyAsync(batch => batch.BatchNumber == batchId, cancellationToken);
    }

    public async Task<BatchNumberListResult> GetBatchNumberListAsync(BatchNumberFilter filter, CancellationToken cancellationToken = default)
    {
        int offset = (filter.PageNumber - 1) * filter.ResultsPerPage;

        IQueryable<Batch> query = _dbContext.Batches.AsNoTracking();
        query = ApplyInventoryItemFilter(query, filter.InventoryItemId);
        query = ApplyQueryActiveStatusFilter(query, filter.IgnoreInactive);

        IQueryable<string> projectedQuery = query
            .Select(batch => batch.BatchNumber)
            .Distinct();

        List<string> queryResult = await projectedQuery
            .Order()
            .Skip(offset)
            .Take(filter.ResultsPerPage)
            .ToListAsync(cancellationToken);

        int totalCount = await _hybridCache.GetOrCreateAsync(
            @$"batch-count-{filter.IgnoreInactive}",
            async cancel => await projectedQuery.CountAsync(cancel),
            BatchCountEntryOptions,
            [@"count:batch"],
            cancellationToken);

        return new BatchNumberListResult(queryResult, filter.PageNumber, queryResult.Count, totalCount);
    }

    public Task<InventoryItem?> GetInventoryItemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _dbContext.InventoryItems
            .FirstOrDefaultAsync(item => item.InventoryItemId == id, cancellationToken);
    }

    private static IQueryable<Batch> ApplyInventoryItemFilter(IQueryable<Batch> query, string? inventoryItemNumber)
    {
        return string.IsNullOrWhiteSpace(inventoryItemNumber)
            ? query
            : query
                .Include(batch => batch.InventoryItem)
                .Where(batch => batch.InventoryItem.InventoryItemId == inventoryItemNumber);
    }

    private static IQueryable<Batch> ApplyQueryActiveStatusFilter(IQueryable<Batch> query, bool ignoreInactive)
    {
        return ignoreInactive
            ? query.Where(line => line.BatchSize == line.IssuedUnits && line.BatchSize == line.IssuedUnits)
            : query;
    }
}
