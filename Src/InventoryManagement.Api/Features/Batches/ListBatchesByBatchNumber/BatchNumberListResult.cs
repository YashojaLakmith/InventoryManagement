namespace InventoryManagement.Api.Features.Batches.ListBatchesByBatchNumber;

public record BatchNumberListResult(
    IReadOnlyCollection<string> BatchNumbers,
    int PageNumber,
    int CountOfResults,
    int TotalCount);