
using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

namespace InventoryManagement.Api.Features.Batches.ListBatchesByBatchNumber;

public class ListBatchNumbersQueryHandler : IRequestHandler<BatchNumberFilter, Result<BatchNumberListResult>>
{
    private readonly IValidator<BatchNumberFilter> _validator;
    private readonly IBatchRepository _batchRepository;
    private readonly ILogger<ListBatchNumbersQueryHandler> _logger;

    public ListBatchNumbersQueryHandler(
        IValidator<BatchNumberFilter> validator,
        ILogger<ListBatchNumbersQueryHandler> logger,
        IBatchRepository batchRepository)
    {
        _validator = validator;
        _logger = logger;
        _batchRepository = batchRepository;
    }

    public async Task<Result<BatchNumberListResult>> Handle(BatchNumberFilter request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        return !validationResult.IsValid
            ? InvalidDataError.CreateFailureResultFromError<BatchNumberListResult>(validationResult.Errors)
            : await _batchRepository.GetBatchNumberListAsync(request, cancellationToken);
    }
}
