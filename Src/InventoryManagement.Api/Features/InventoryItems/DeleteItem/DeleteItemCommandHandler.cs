using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.DeleteItem;

public class DeleteItemCommandHandler : IRequestHandler<ItemIdToDelete, Result>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ItemIdToDelete> _validator;
    private readonly ILogger<DeleteItemCommandHandler> _logger;

    public DeleteItemCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        IUnitOfWork unitOfWork,
        IValidator<ItemIdToDelete> validator,
        ILogger<DeleteItemCommandHandler> logger)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> Handle(ItemIdToDelete request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        InventoryItem? existingItem = await _inventoryItemRepository.GetInventoryItemByIdAsync(request.ItemId, cancellationToken);
        if (existingItem is null)
        {
            return Result.Fail(new NotFoundError(@$"Inventory item with id: {request.ItemId}"));
        }

        await RemoveFromDatabaseAsync(existingItem, cancellationToken);

        return Result.Ok();
    }

    private Task<int> RemoveFromDatabaseAsync(InventoryItem existingItem, CancellationToken cancellationToken)
    {
        _inventoryItemRepository.DeleteItem(existingItem);
        return _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
