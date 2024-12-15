using InventoryManagement.Api.Features.InventoryItems;

namespace InventoryManagement.Api.Features.Batches;

public class Batch
{
    public string BatchNumber { get; private init; }
    public InventoryItem InventoryItem { get; private init; }
    public int BatchSize { get; private init; }
    public decimal CostPerUnit { get; private init; }
    public int AvailableUnits { get; set; }

    private Batch() { }

    public static Batch Create(
        string batchNumber,
        InventoryItem inventoryItem,
        int batchSize,
        decimal costPerUnit)
    {
        return new(batchNumber, inventoryItem, batchSize, costPerUnit);
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
