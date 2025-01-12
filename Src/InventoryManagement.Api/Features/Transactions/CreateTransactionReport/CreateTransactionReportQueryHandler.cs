using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

namespace InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

public class CreateTransactionReportQueryHandler : IRequestHandler<TransactionReportFilters, Result<TransactionReportStream>>
{
    private readonly ITransactionReportGenerator _reportGenerator;
    private readonly IValidator<TransactionReportFilters> _validator;
    private readonly ILogger<CreateTransactionReportQueryHandler> _logger;

    public CreateTransactionReportQueryHandler(
        ITransactionReportGenerator reportGenerator,
        IValidator<TransactionReportFilters> validator,
        ILogger<CreateTransactionReportQueryHandler> logger)
    {
        _reportGenerator = reportGenerator;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<TransactionReportStream>> Handle(TransactionReportFilters request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        return !validationResult.IsValid
            ? InvalidDataError.CreateFailureResultFromError<TransactionReportStream>(validationResult.Errors)
            : await _reportGenerator.GenerateInventoryTransactionReportAsync(request, cancellationToken);
    }
}
