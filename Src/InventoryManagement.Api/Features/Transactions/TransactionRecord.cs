using FluentResults;

using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.InventoryItems;
using InventoryManagement.Api.Features.Transactions.TransactionErrors;

namespace InventoryManagement.Api.Features.Transactions;

public class TransactionRecord
{
    public long RecordId { get; private init; }
    public Batch BatchOfItem { get; private init; }
    public InventoryItem InventoryItem { get; private init; }
    public int TransactionUnitCount { get; private init; }
    public DateTime Timestamp { get; private init; }

    private TransactionRecord() { }

    public static Result<TransactionRecord> CreateGoodsReceiveTransaction(Batch batch, int itemCount)
    {
        if (itemCount + batch.ReceivedUnits > batch.BatchSize)
        {
            return SurplusQuantityError.CreateFailureResultFromError<TransactionRecord>();
        }
        
        batch.ReceivedUnits += itemCount;
        return new TransactionRecord(batch, batch.InventoryItem, itemCount, DateTime.UtcNow);
    }

    public static Result<TransactionRecord> CreateGoodsIssuanceTransaction(Batch batch, int itemCount)
    {
        if (itemCount + batch.IssuedUnits > batch.ReceivedUnits)
        {
            return InsufficientQuantityError.CreateFailureResultFromError<TransactionRecord>(itemCount);
        }
        
        batch.IssuedUnits += itemCount;
        return new TransactionRecord(batch, batch.InventoryItem, -itemCount, DateTime.UtcNow);
    }

    private TransactionRecord(
        Batch batch,
        InventoryItem inventoryItem,
        int transactionCount,
        DateTime timestamp)
    {
        BatchOfItem = batch;
        InventoryItem = inventoryItem;
        TransactionUnitCount = transactionCount;
        Timestamp = timestamp;
    }
}
