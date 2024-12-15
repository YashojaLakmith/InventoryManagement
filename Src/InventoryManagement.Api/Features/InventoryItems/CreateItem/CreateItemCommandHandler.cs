using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Features.InventoryItems.Errors;
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

        ValidationResult validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            IEnumerable<string> errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Fail<string>(new InvalidNewItemInformationError(errorMessages));
        }

        bool isItemIdInUse = await _dbContext.InventoryItems.AnyAsync(item => item.InventoryItemId == request.ItemId, cancellationToken);
        if (isItemIdInUse)
        {
            throw new NotImplementedException();
        }

        InventoryItem newItem = InventoryItem.Create(
            request.ItemId,
            request.ItemName,
            request.MeasurementUnit);

        _dbContext.Add(newItem);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(newItem.InventoryItemId);
    }
}
