using FluentValidation;

using InventoryManagement.Api.Features.Shared.Abstractions;

namespace InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

public class TransactionReportFilterValidator : AbstractValidator<TransactionReportFilters>
{
    public TransactionReportFilterValidator(ITimeProvider timeProvider)
    {
        RuleFor(info => info.SinceDate)
            .NotNull()
            .WithMessage(@"From date is required.")
            .LessThanOrEqualTo(timeProvider.CurrentUtcTime)
            .WithMessage(@"From date must not be a future date.");

        RuleFor(info => info.ToDate)
            .NotNull()
            .WithMessage(@"To date is required.")
            .GreaterThan(timeProvider.CurrentUtcTime)
            .WithMessage(@"From date must not be a past date.");

        RuleFor(info => info.TransactionTypes)
            .NotNull()
            .WithMessage(@"Transaction types should not be null.");

        RuleForEach(info => info.TransactionTypes)
            .NotEmpty()
            .WithMessage(@"Transaction type name should not be null or empty.")
            .Must(name =>
                name.Equals(InventoryTransactionTypes.Issue, StringComparison.OrdinalIgnoreCase)
                || name.Equals(InventoryTransactionTypes.Receive, StringComparison.OrdinalIgnoreCase))
            .WithMessage(@"Invalid transaction type.");
    }
}
