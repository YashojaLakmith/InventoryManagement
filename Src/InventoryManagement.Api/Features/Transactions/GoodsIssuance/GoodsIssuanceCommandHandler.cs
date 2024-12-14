using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Transactions.GoodsIssuance;

public class GoodsIssuanceCommandHandler : IRequestHandler<IssuanceInformation, Result>
{
    public Task<Result> Handle(IssuanceInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        throw new NotImplementedException();
    }
}
