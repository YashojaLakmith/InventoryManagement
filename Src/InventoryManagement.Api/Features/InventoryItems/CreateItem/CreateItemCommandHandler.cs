
using FluentResults;

using FluentValidation.Results;

using InventoryManagement.Api.Features.InventoryItems.Errors;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<NewItemInformation, Result<string>>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateItemCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<string>> Handle(NewItemInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        NewItemInformationValidator validator = new();
        ValidationResult validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            IEnumerable<string> errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Fail<string>(new InvalidNewItemInformationError(errorMessages));
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
