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
            IEnumerable<string> errorMessages = validationResult.Errors.Select(error => error.ErrorMessage);
            InvalidDataError invalidDataError = new(errorMessages);
            return Result.Fail(invalidDataError);
        }

        bool isBatchNumberExist = await _dbContext.Batches.AnyAsync(batch => batch.BatchNumber == request.BatchNumber, cancellationToken);
        if (isBatchNumberExist)
        {
            AlreadyExistsError alreadyExistsError = new(@"");
        }

        List<Batch> batches = [];

        foreach (ItemOrder order in request.ItemOrders)
        {
            InventoryItem? existingItem = await _dbContext.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.InventoryItemId == order.ItemId, cancellationToken);

            if (existingItem is null)
            {
                throw new NotImplementedException();
            }

            Batch newBatch = Batch.Create(request.BatchNumber, existingItem, order.BatchSize, order.CostPerUnit);
            batches.Add(newBatch);
        }

        _dbContext.Batches.AddRange(batches);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
