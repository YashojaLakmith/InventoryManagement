
using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.InventoryItems;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Infrastructure.Database.Repositories;

public class BatchRepository : IBatchRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BatchRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public Task<InventoryItem?> GetInventoryItemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _dbContext.InventoryItems
            .FirstOrDefaultAsync(item => item.InventoryItemId == id, cancellationToken);
    }
}
