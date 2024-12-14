using FluentResults;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public class GoodReceiveCommandHandler : IRequestHandler<RetrievalInformation, Result>
{
    private readonly ApplicationDbContext _dbContext;

    public GoodReceiveCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(RetrievalInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        User? receivingUser = await _dbContext.Users.FindAsync(request.UserEmail);
        if (receivingUser is null)
        {
            return Result.Fail(@"BLah");
        }

        RetrievalInformationValidator validator = new();
        ValidationResult validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return Result.Fail(@"Blah");
        }

        Batch? existingBatch = await _dbContext.Batches
            .AsNoTracking()
            .Include(batch => batch.InventoryItem)
            .Where(batch => batch.BatchNumber == request.BatchNumber && batch.InventoryItem.InventoryItemId == request.InventoryItemNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingBatch is null)
        {
            return Result.Fail(@"Blah");
        }

        Result<TransactionRecord> newTransactionResult = TransactionRecord.CreateGoodsReceiveTransaction(existingBatch, receivingUser, request.ItemCount);
        if (newTransactionResult.IsFailed)
        {
            return Result.Fail(newTransactionResult.Errors);
        }

        _dbContext.TransactionRecords.Add(newTransactionResult.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
