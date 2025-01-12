using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;

namespace InventoryManagement.Api.Features.Batches.DeleteBatchLine;

public class DeleteBatchLineCommandHandler : IRequestHandler<DeleteBatchLineCommand, Result>
{
    private readonly IValidator<DeleteBatchLineCommand> _validator;
    private readonly ILogger<DeleteBatchLineCommandHandler> _logger;
    private readonly IBatchRepository _batchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _hybridCache;

    public DeleteBatchLineCommandHandler(
        IValidator<DeleteBatchLineCommand> validator,
        ILogger<DeleteBatchLineCommandHandler> logger,
        IBatchRepository batchRepository,
        IUnitOfWork unitOfWork,
        HybridCache hybridCache)
    {
        _validator = validator;
        _logger = logger;
        _batchRepository = batchRepository;
        _unitOfWork = unitOfWork;
        _hybridCache = hybridCache;
    }

    public async Task<Result> Handle(DeleteBatchLineCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        Batch? existingBatch = await _batchRepository.GetBatchLineAsync(request.BatchNumber, request.ItemNumber, cancellationToken);
        if (existingBatch is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"Batch line with item Id: {request.ItemNumber} and batch number: {request.BatchNumber}");
        }

        if (HasBatchLineInvolvedInTransaction(existingBatch))
        {
            return ActionNotAllowedError.CreateFailureResultFromError(@"This batch line cannot be deleted. It has already involved in transactions.");
        }

        _batchRepository.RemoveBatchLine(existingBatch);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveByTagAsync(@"count:batch", cancellationToken);

        return Result.Ok();
    }

    private static bool HasBatchLineInvolvedInTransaction(Batch existingBatch) =>
            existingBatch.IssuedUnits != 0 || existingBatch.ReceivedUnits != 0;
}
