
using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.Transactions;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Infrastructure.Database.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TransactionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddTransactionRecord(TransactionRecord transactionRecord)
    {
        _dbContext.TransactionRecords.Add(transactionRecord);
    }

    public Task<Batch?> GetBatchLineByIdsAsync(string inventoryItemId, string batchId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Batches
            .Include(batch => batch.InventoryItem)
            .FirstOrDefaultAsync(batch =>
                batch.BatchNumber == batchId
                && batch.InventoryItem.InventoryItemId == inventoryItemId,
                cancellationToken);
    }
}
