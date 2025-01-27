namespace InventoryManagement.Api.Features.InventoryItems.ListItems;

public record ListItemsResult(IReadOnlyCollection<ListItem> Items, int NumberOfRecords, int PageNumber, int TotalNumberOfRecords);