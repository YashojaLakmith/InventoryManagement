using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Batches.ListBatchesByBatchNumber;

public record BatchNumberFilter(
    string? InventoryItemId,
    bool IgnoreInactive,
    int PageNumber,
    int ResultsPerPage) : IRequest<Result<BatchNumberListResult>>;