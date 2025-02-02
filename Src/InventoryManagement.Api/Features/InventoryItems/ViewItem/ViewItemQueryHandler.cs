using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.ViewItem;

public class ViewItemQueryHandler : IRequestHandler<ViewItemQuery, Result<ItemDetails>>
{
    private readonly IValidator<ViewItemQuery> _validator;
    private readonly ILogger<ViewItemQueryHandler> _logger;
    private readonly IInventoryItemRepository _inventoryItemRepository;

    public ViewItemQueryHandler(
        IValidator<ViewItemQuery> validator,
        ILogger<ViewItemQueryHandler> logger,
        IInventoryItemRepository inventoryItemRepository)
    {
        _validator = validator;
        _logger = logger;
        _inventoryItemRepository = inventoryItemRepository;
    }

    public async Task<Result<ItemDetails>> Handle(ViewItemQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError<ItemDetails>(validationResult.Errors);
        }

        ItemDetails? itemDetails = await _inventoryItemRepository.GetItemDetailsByIdAsync(request.ItemId, cancellationToken);

        return itemDetails is null
            ? NotFoundError.CreateFailureResultFromError<ItemDetails>($@"Item with Id: {request.ItemId}")
            : (Result<ItemDetails>)itemDetails;
    }
}
