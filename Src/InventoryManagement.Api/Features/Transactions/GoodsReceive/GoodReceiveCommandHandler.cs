using FluentResults;

using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.Transactions.TransactionErrors;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public class GoodReceiveCommandHandler : IRequestHandler<RetrievalInformation, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IValidator<RetrievalInformation> _validator;
    private readonly ILogger<GoodReceiveCommandHandler> _logger;

    public GoodReceiveCommandHandler(
        ApplicationDbContext dbContext,
        IValidator<RetrievalInformation> validator,
        ILogger<GoodReceiveCommandHandler> logger)
    {
        _dbContext = dbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> Handle(RetrievalInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        const int maximumAttempts = 5;
        for (int i = 0; i < maximumAttempts; i++)
        {
            try
            {
                return await TryExecutingRetrievalAsync(request, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _dbContext.ChangeTracker.Clear();
                _logger.LogInformation(@"Concurrency violation. Trying to execute transaction using updated data.");
            }
        }

        return SurplusQuantityError.CreateFailureResultFromError();
    }

    private async Task<Result> TryExecutingRetrievalAsync(RetrievalInformation request, CancellationToken cancellationToken)
    {
        Batch? existingBatch = await _dbContext.Batches
            .Include(batch => batch.InventoryItem)
            .FirstOrDefaultAsync(
                batch => batch.BatchNumber == request.BatchNumber && batch.InventoryItem.InventoryItemId == request.InventoryItemNumber, cancellationToken);

        if (existingBatch is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"Batch with number: {request.BatchNumber}");
        }

        Result<TransactionRecord> newTransactionResult = TransactionRecord.CreateGoodsReceiveTransaction(existingBatch, request.ItemCount);
        if (newTransactionResult.IsFailed)
        {
            return SurplusQuantityError.CreateFailureResultFromError();
        }

        await UpdateDatabase(newTransactionResult, cancellationToken);

        return Result.Ok();
    }

    private Task<int> UpdateDatabase(Result<TransactionRecord> newTransactionResult, CancellationToken cancellationToken)
    {
        _dbContext.TransactionRecords.Add(newTransactionResult.Value);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
