using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Transactions.GoodsIssuance;

public record IssuanceInformation(
    string ItemId,
    string BatchNumber,
    int NumberOfItemsToIssue,
    string IssuerEmail
    ) : IRequest<Result>;