namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public record RetrievalDto(
    string BatchNumber,
    string ItemId,
    int CountToRetrieve);