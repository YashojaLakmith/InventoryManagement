namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public record ItemOrder(
    string ItemId,
    int BatchSize,
    decimal CostPerUnit);