using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public record RetrievalInformation(
    string InventoryItemNumber,
    string BatchNumber,
    int ItemCount) : IRequest<Result>;