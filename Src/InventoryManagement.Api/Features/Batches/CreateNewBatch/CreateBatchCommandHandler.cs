using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.InventoryItems;

using MediatR;

namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public class CreateBatchCommandHandler : IRequestHandler<NewBatchInformation, Result>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<NewBatchInformation> _validator;
    private readonly ILogger<CreateBatchCommandHandler> _logger;

    public CreateBatchCommandHandler(
        IBatchRepository batchRepository,
        IUnitOfWork unitOfWork,
        IValidator<NewBatchInformation> validator,
        ILogger<CreateBatchCommandHandler> logger)
    {
        _batchRepository = batchRepository;
        _unitOfWork = unitOfWork;
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

        bool isBatchNumberExist = await _batchRepository.DoesBatchExistAsync(request.BatchNumber, cancellationToken);
        if (isBatchNumberExist)
        {
            return AlreadyExistsError.CreateFailureResultFromError($@"Batch with Id: {request.BatchNumber}");
        }

        List<Batch> batches = [];

        foreach (ItemOrder order in request.ItemOrders)
        {
            InventoryItem? existingItem = await _batchRepository.GetInventoryItemByIdAsync(order.ItemId, cancellationToken);

            if (existingItem is null)
            {
                _unitOfWork.ClearChanges();
                return NotFoundError.CreateFailureResultFromError($@"Inventory item with Id: {order.ItemId}");
            }

            Batch newBatch = Batch.Create(request.BatchNumber, existingItem, order.BatchSize, order.CostPerUnit);
            batches.Add(newBatch);
        }

        _batchRepository.AddBatchLines(batches);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
