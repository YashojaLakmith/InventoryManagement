using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Transactions.GoodsIssuance;

public class GoodsIssuanceCommandHandler : IRequestHandler<IssuanceInformation, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IValidator<IssuanceInformation> _validator;
    private readonly ILogger<GoodsIssuanceCommandHandler> _logger;

    public GoodsIssuanceCommandHandler(
        ApplicationDbContext dbContext,
        IValidator<IssuanceInformation> validator,
        ILogger<GoodsIssuanceCommandHandler> logger)
    {
        _dbContext = dbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> Handle(IssuanceInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new NotImplementedException();
        }

        Batch? existingBatch = await _dbContext.Batches
            .Include(batch => batch.InventoryItem)
            .FirstOrDefaultAsync(
            batch => batch.BatchNumber == request.BatchNumber && batch.InventoryItem.InventoryItemId == request.ItemId, cancellationToken);

        if (existingBatch is null)
        {
            throw new NotImplementedException();
        }

        Result<TransactionRecord> newTransaction = TransactionRecord.CreateGoodsIssuanceTransaction(existingBatch, request.NumberOfItemsToIssue);
        if (!newTransaction.IsSuccess)
        {
            throw new NotImplementedException();
        }

        _dbContext.TransactionRecords.Add(newTransaction.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
