namespace InventoryManagement.Api.Features.InventoryItems;

public class InventoryItem
{
    public string InventoryItemId { get; private init; }
    public string ItemName { get; private set; }
    public string MeasurementUnit { get; private init; }

    private InventoryItem() { }

    public static InventoryItem Create(
        string itemId,
        string itemName,
        string measurementUnit)
    {
        return new InventoryItem(itemId, itemName, measurementUnit);
    }

    private InventoryItem(
        string itemId,
        string itemname,
        string measurementUnit)
    {
        InventoryItemId = itemId;
        ItemName = itemname;
        MeasurementUnit = measurementUnit;
    }
}
