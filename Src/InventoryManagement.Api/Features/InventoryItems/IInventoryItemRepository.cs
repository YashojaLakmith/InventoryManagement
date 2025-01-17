using InventoryManagement.Api.Features.InventoryItems.ViewItem;

namespace InventoryManagement.Api.Features.InventoryItems;

public interface IInventoryItemRepository
{
    Task<bool> IsInventoryItemIdInUseAsync(string inventoryItemId, CancellationToken cancellationToken = default);
    void CreateNewItem(InventoryItem newItem);
    Task<InventoryItem?> GetInventoryItemByIdAsync(string inventoryItemId, CancellationToken cancellationToken = default);
    void DeleteItem(InventoryItem existingItem);
    Task<ItemDetails?> GetItemDetailsByIdAsync(string itemId, CancellationToken cancellationToken = default);
}
