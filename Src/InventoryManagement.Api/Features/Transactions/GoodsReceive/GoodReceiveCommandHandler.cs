using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Features.Batches;
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
            throw new NotImplementedException();
        }

        Batch? existingBatch = await _dbContext.Batches
            .Include(batch => batch.InventoryItem)
            .FirstOrDefaultAsync(
            batch => batch.BatchNumber == request.BatchNumber && batch.InventoryItem.InventoryItemId == request.InventoryItemNumber, cancellationToken);

        if (existingBatch is null)
        {
            throw new NotImplementedException();
        }

        Result<TransactionRecord> newTransactionResult = TransactionRecord.CreateGoodsReceiveTransaction(existingBatch, request.ItemCount);
        if (newTransactionResult.IsFailed)
        {
            throw new NotImplementedException();
        }

        _dbContext.TransactionRecords.Add(newTransactionResult.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
