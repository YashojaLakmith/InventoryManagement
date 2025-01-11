using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace InventoryManagement.Api.Features.Batches.DeleteBatchLine;

public class DeleteBatchLineCommandHandler : IRequestHandler<DeleteBatchLineCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IValidator<DeleteBatchLineCommand> _validator;
    private readonly ILogger<DeleteBatchLineCommandHandler> _logger;
    private readonly HybridCache _hybridCache;

    public DeleteBatchLineCommandHandler(
        ApplicationDbContext dbContext,
        IValidator<DeleteBatchLineCommand> validator,
        ILogger<DeleteBatchLineCommandHandler> logger,
        HybridCache hybridCache)
    {
        _dbContext = dbContext;
        _validator = validator;
        _logger = logger;
        _hybridCache = hybridCache;
    }

    public async Task<Result> Handle(DeleteBatchLineCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        Batch? existingBatch = await _dbContext.Batches
            .Include(batch => batch.InventoryItem)
            .AsNoTracking()
            .FirstOrDefaultAsync(
            batch => batch.BatchNumber == request.BatchNumber && batch.InventoryItem.InventoryItemId == request.ItemNumber, cancellationToken);

        if (existingBatch is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"Batch line with item Id: {request.ItemNumber} and batch number: {request.BatchNumber}");
        }

        bool hasAnyTransactions = await _dbContext.TransactionRecords
            .Include(record => record.InventoryItem)
            .Include(record => record.BatchOfItem)
            .AsNoTracking()
            .AnyAsync(
            record =>
                record.InventoryItem.InventoryItemId == request.ItemNumber
                && record.BatchOfItem.BatchNumber == request.BatchNumber
                , cancellationToken);

        if (hasAnyTransactions)
        {
            return ActionNotAllowedError.CreateFailureResultFromError(@"This batch line cannot be deleted. It has already involved in transactions.");
        }

        _dbContext.Batches.Remove(existingBatch);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveByTagAsync(@"count:batch", cancellationToken);
        return Result.Ok();
    }
}
