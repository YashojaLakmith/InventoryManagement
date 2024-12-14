namespace InventoryManagement.Api.Features.Transactions.GoodsIssuance;

public record GoodsIssueDto(
    string ItemId,
    string BatchNumber,
    int NumberOfItemsToIssue);