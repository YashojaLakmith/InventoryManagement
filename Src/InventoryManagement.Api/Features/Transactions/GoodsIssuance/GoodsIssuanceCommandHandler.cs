using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Exceptions;
using InventoryManagement.Api.Features.Batches;
using InventoryManagement.Api.Features.Transactions.TransactionErrors;

using MediatR;

namespace InventoryManagement.Api.Features.Transactions.GoodsIssuance;

public class GoodsIssuanceCommandHandler : IRequestHandler<IssuanceInformation, Result>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<IssuanceInformation> _validator;
    private readonly ILogger<GoodsIssuanceCommandHandler> _logger;

    public GoodsIssuanceCommandHandler(
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        IValidator<IssuanceInformation> validator,
        ILogger<GoodsIssuanceCommandHandler> logger)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> Handle(IssuanceInformation request, CancellationToken cancellationToken)
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
                return await TryExecutingTransactionAsync(request, cancellationToken);
            }
            catch (ConcurrencyViolationException)
            {
                _unitOfWork.ClearChanges();
                _logger.LogInformation(@"Concurrency violation. Trying to execute transaction using updated data.");
            }
        }

        return ServerBusyError.CreateFailureResultFromError();
    }

    private async Task<Result> TryExecutingTransactionAsync(IssuanceInformation request, CancellationToken cancellationToken)
    {
        Batch? existingBatch = await _transactionRepository.GetBatchLineByIdsAsync(request.ItemId, request.BatchNumber, cancellationToken);

        if (existingBatch is null)
        {
            return NotFoundError.CreateFailureResultFromError(@$"Batch with number: {request.BatchNumber}");
        }

        Result<TransactionRecord> newTransaction = TransactionRecord.CreateGoodsIssuanceTransaction(existingBatch, request.NumberOfItemsToIssue);
        if (!newTransaction.IsSuccess)
        {
            return InsufficientQuantityError.CreateFailureResultFromError(request.NumberOfItemsToIssue);
        }

        await SaveChangesToDatabaseAsync(newTransaction.Value, cancellationToken);
        return Result.Ok();
    }

    private Task<int> SaveChangesToDatabaseAsync(TransactionRecord newTransaction, CancellationToken cancellationToken)
    {
        _transactionRepository.AddTransactionRecord(newTransaction);
        return _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
