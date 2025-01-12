using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Exceptions;
using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.Transactions.TransactionErrors;

using MediatR;

namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public class GoodReceiveCommandHandler : IRequestHandler<RetrievalInformation, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IValidator<RetrievalInformation> _validator;
    private readonly ILogger<GoodReceiveCommandHandler> _logger;

    public GoodReceiveCommandHandler(
        IUnitOfWork unitOfWork,
        ITransactionRepository transactionRepository,
        IValidator<RetrievalInformation> validator,
        ILogger<GoodReceiveCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _transactionRepository = transactionRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> Handle(RetrievalInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        const int maximumAttempts = 5;
        for (int i = 0; i < maximumAttempts; i++)
        {
            try
            {
                return await TryExecutingRetrievalAsync(request, cancellationToken);
            }
            catch (ConcurrencyViolationException)
            {
                _unitOfWork.ClearChanges();
                _logger.LogInformation(@"Concurrency violation. Trying to execute transaction using updated data.");
            }
        }

        return SurplusQuantityError.CreateFailureResultFromError();
    }

    private async Task<Result> TryExecutingRetrievalAsync(RetrievalInformation request, CancellationToken cancellationToken)
    {
        Batch? existingBatch = await _transactionRepository.GetBatchLineByIdsAsync(request.InventoryItemNumber, request.BatchNumber, cancellationToken);

        if (existingBatch is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"Batch with number: {request.BatchNumber}");
        }

        Result<TransactionRecord> newTransactionResult = TransactionRecord.CreateGoodsReceiveTransaction(existingBatch, request.ItemCount);
        if (newTransactionResult.IsFailed)
        {
            return SurplusQuantityError.CreateFailureResultFromError();
        }

        await UpdateDatabase(newTransactionResult.Value, cancellationToken);

        return Result.Ok();
    }

    private Task<int> UpdateDatabase(TransactionRecord newTransactionRecord, CancellationToken cancellationToken)
    {
        _transactionRepository.AddTransactionRecord(newTransactionRecord);
        return _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
