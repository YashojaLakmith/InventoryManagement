using FluentResults;
using InventoryManagement.Api.Features.InventoryItems;
using InventoryManagement.Api.Features.Transactions;

namespace InventoryManagement.Api.Features.Batches;

public class Batch
{
    public string BatchNumber { get; private init; }
    public InventoryItem InventoryItem { get; private init; }
    public int BatchSize { get; private init; }
    public decimal CostPerUnit { get; private init; }
    public int ReceivedUnits { get; set; }
    public int IssuedUnits { get; set; }

    private Batch() { }

    public static Batch Create(
        string batchNumber,
        InventoryItem inventoryItem,
        int batchSize,
        decimal costPerUnit)
    {
        return new Batch(batchNumber, inventoryItem, batchSize, costPerUnit);
    }

    private Batch(
        string batchNumber,
        InventoryItem inventoryItem,
        int batchSize,
        decimal costPerUnit)
    {
        BatchNumber = batchNumber;
        InventoryItem = inventoryItem;
        BatchSize = batchSize;
        CostPerUnit = costPerUnit;
    }
}
