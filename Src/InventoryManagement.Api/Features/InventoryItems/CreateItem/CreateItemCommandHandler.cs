using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<NewItemInformation, Result<string>>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateItemCommandHandler> _logger;
    private readonly IValidator<NewItemInformation> _validator;

    public CreateItemCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateItemCommandHandler> logger,
        IValidator<NewItemInformation> validator)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<string>> Handle(NewItemInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError<string>(validationResult.Errors);
        }

        if (await _inventoryItemRepository.IsInventoryItemIdInUseAsync(request.ItemId, cancellationToken))
        {
            return AlreadyExistsError.CreateFailureResultFromError<string>($@"Item with Id: {request.ItemId}");
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
        _inventoryItemRepository.CreateNewItem(newItem);
        return _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
