using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public record NewBatchInformation(
    string BatchNumber,
    IReadOnlyCollection<ItemOrder> ItemOrders,
    string UserEmail) : IRequest<Result>;