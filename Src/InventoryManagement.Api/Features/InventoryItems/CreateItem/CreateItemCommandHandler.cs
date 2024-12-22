using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.InventoryItems.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<NewItemInformation, Result<string>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IValidator<NewItemInformation> _validator;

    public CreateItemCommandHandler(ApplicationDbContext dbContext, IValidator<NewItemInformation> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result<string>> Handle(NewItemInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            IEnumerable<string> errorMessages = validationResult.Errors
                .Select(e => e.ErrorMessage);
            return Result.Fail<string>(new InvalidDataError(errorMessages));
        }

        bool isItemIdInUse = await _dbContext.InventoryItems
            .AnyAsync(item => item.InventoryItemId == request.ItemId, cancellationToken);
        if (isItemIdInUse)
        {
            return Result.Fail<string>(new AlreadyExistsError($@"{request.ItemId}"));
        }

        InventoryItem newItem = InventoryItem.Create(
            request.ItemId,
            request.ItemName,
            request.MeasurementUnit);

        await AddToDatabaseAsync(newItem, cancellationToken);

        return Result.Ok(newItem.InventoryItemId);
    }

    private Task<int> AddToDatabaseAsync(InventoryItem newItem, CancellationToken cancellationToken)
    {
        _dbContext.Add(newItem);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
