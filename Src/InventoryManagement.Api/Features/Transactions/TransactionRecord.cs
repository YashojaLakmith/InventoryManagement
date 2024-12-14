using FluentResults;

using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.InventoryItems;
using InventoryManagement.Api.Features.Users;

namespace InventoryManagement.Api.Features.Transactions;

public class TransactionRecord
{
    public long RecordId { get; private init; }
    public Batch BatchOfItem { get; private init; }
    public InventoryItem InventoryItem { get; private init; }
    public User TransactionDoneBy { get; private init; }
    public int TransactionUnitCount { get; private init; }
    public DateTime Timestamp { get; private init; }

    private TransactionRecord() { }

    public static Result<TransactionRecord> CreateGoodsReceiveTransaction(Batch batch, User transactionDoneBy, int itemCount)
    {
        batch.AvailableUnits += itemCount;
        return new TransactionRecord(batch, transactionDoneBy, batch.InventoryItem, itemCount, DateTime.UtcNow);
    }

    public static Result<TransactionRecord> CreateGoodsIssuanceTransaction(Batch batch, User transactionDoneBy, int itemCount)
    {
        batch.AvailableUnits -= itemCount;
        return new TransactionRecord(batch, transactionDoneBy, batch.InventoryItem, itemCount, DateTime.UtcNow);
    }

    private TransactionRecord(
        Batch batch,
        User receiveDoneBy,
        InventoryItem inventoryItem,
        int retrievedCount,
        DateTime timestamp)
    {
        BatchOfItem = batch;
        TransactionDoneBy = receiveDoneBy;
        InventoryItem = inventoryItem;
        TransactionUnitCount = retrievedCount;
        Timestamp = timestamp;
    }
}
