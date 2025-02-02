namespace InventoryManagement.Api.Features.InventoryItems.ViewItem;

public record ItemDetails(
    string ItemId,
    string ItemName,
    string MeasurementUnit,
    int AvailableQuantity,
    decimal WeightedAverageCostPerUnit);
