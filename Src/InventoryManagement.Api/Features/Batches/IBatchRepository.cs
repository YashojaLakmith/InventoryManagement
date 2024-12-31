using InventoryManagement.Api.Features.InventoryItems;

namespace InventoryManagement.Api.Features.Batches;

public interface IBatchRepository
{
    Task<bool> DoesBatchExistAsync(string batchId, CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetInventoryItemByIdAsync(string id, CancellationToken cancellationToken = default);
    void AddBatchLines(IReadOnlyCollection<Batch> batchLines);
}
