using FluentResults;

using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.Transactions.TransactionErrors;
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
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        const int maximumAttempts = 5;
        for (int i = 0; i < maximumAttempts; i++)
        {
            try
            {
                return await TryExecutingTransactionAsync(request, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _dbContext.ChangeTracker.Clear();
                _logger.LogInformation(@"Concurrency violation. Trying to execute transaction using updated data.");
            }
        }

        return ServerBusyError.CreateFailureResultFromError();
    }

    private async Task<Result> TryExecutingTransactionAsync(IssuanceInformation request, CancellationToken cancellationToken)
    {
        Batch? existingBatch = await _dbContext.Batches
            .Include(batch => batch.InventoryItem)
            .FirstOrDefaultAsync(
                batch => batch.BatchNumber == request.BatchNumber
                         && batch.InventoryItem.InventoryItemId == request.ItemId, cancellationToken);

        if (existingBatch is null)
        {
            return NotFoundError.CreateFailureResultFromError(@$"Batch with number: {request.BatchNumber}");
        }

        Result<TransactionRecord> newTransaction = TransactionRecord.CreateGoodsIssuanceTransaction(existingBatch, request.NumberOfItemsToIssue);
        if (!newTransaction.IsSuccess)
        {
            return InsufficientQuantityError.CreateFailureResultFromError(request.NumberOfItemsToIssue);
        }

        await SaveChangesToDatabaseAsync(newTransaction.Value, cancellationToken);
        return Result.Ok();
    }

    private Task<int> SaveChangesToDatabaseAsync(TransactionRecord newTransaction, CancellationToken cancellationToken)
    {
        _dbContext.TransactionRecords.Add(newTransaction);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
