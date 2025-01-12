using InventoryManagement.Api.Features.Batches;

namespace InventoryManagement.Api.Features.Transactions;

public interface ITransactionRepository
{
    void AddTransactionRecord(TransactionRecord transactionRecord);
    Task<Batch?> GetBatchLineByIdsAsync(string inventoryItemId, string batchId, CancellationToken cancellationToken = default);
}
