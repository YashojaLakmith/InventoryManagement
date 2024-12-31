using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.InventoryItems;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public class CreateBatchCommandHandler : IRequestHandler<NewBatchInformation, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IValidator<NewBatchInformation> _validator;
    private readonly ILogger<CreateBatchCommandHandler> _logger;

    public CreateBatchCommandHandler(
        ApplicationDbContext dbContext,
        IValidator<NewBatchInformation> validator,
        ILogger<CreateBatchCommandHandler> logger)
    {
        _dbContext = dbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> Handle(NewBatchInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        bool isBatchNumberExist = await _dbContext.Batches.AnyAsync(batch => batch.BatchNumber == request.BatchNumber, cancellationToken);
        if (isBatchNumberExist)
        {
            return AlreadyExistsError.CreateFailureResultFromError($@"Batch with Id: {request.BatchNumber}");
        }

        List<Batch> batches = [];

        foreach (ItemOrder order in request.ItemOrders)
        {
            InventoryItem? existingItem = await _dbContext.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.InventoryItemId == order.ItemId, cancellationToken);

            if (existingItem is null)
            {
                _dbContext.ChangeTracker.Clear();
                return NotFoundError.CreateFailureResultFromError($@"Inventory item with Id: {order.ItemId}");
            }

            Batch newBatch = Batch.Create(request.BatchNumber, existingItem, order.BatchSize, order.CostPerUnit);
            batches.Add(newBatch);
        }

        _dbContext.Batches.AddRange(batches);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
