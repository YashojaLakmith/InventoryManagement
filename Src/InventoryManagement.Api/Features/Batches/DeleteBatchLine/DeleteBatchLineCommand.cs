using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Batches.DeleteBatchLine;

public record DeleteBatchLineCommand(
    string BatchNumber,
    string ItemNumber) : IRequest<Result>;