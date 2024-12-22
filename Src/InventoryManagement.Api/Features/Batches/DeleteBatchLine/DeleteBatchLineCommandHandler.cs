using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Batches.DeleteBatchLine;

public class DeleteBatchLineCommandHandler : IRequestHandler<DeleteBatchLineCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IValidator<DeleteBatchLineCommand> _validator;
    private readonly ILogger<DeleteBatchLineCommandHandler> _logger;

    public DeleteBatchLineCommandHandler(
        ApplicationDbContext dbContext,
        IValidator<DeleteBatchLineCommand> validator,
        ILogger<DeleteBatchLineCommandHandler> logger)
    {
        _dbContext = dbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteBatchLineCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new NotImplementedException();
        }

        Batch? existingBatch = await _dbContext.Batches
            .Include(batch => batch.InventoryItem)
            .AsNoTracking()
            .FirstOrDefaultAsync(
            batch => batch.BatchNumber == request.BatchNumber && batch.InventoryItem.InventoryItemId == request.ItemNumber, cancellationToken);

        if (existingBatch is null)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        _dbContext.Batches.Remove(existingBatch);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
