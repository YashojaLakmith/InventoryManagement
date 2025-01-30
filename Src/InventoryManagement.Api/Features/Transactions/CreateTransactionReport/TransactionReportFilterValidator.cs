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
            .LessThan(timeProvider.CurrentUtcTime)
            .WithMessage(@"From date must not be a future date.");

        RuleFor(info => info.ToDate)
            .NotNull()
            .WithMessage(@"To date is required.")
            .LessThanOrEqualTo(timeProvider.CurrentUtcTime)
            .WithMessage(@"To date must not be a future date.");

        RuleFor(info => info.SinceDate < info.ToDate)
            .Equal(true)
            .WithMessage("To date must be greate than since date.");

        RuleFor(info => info.TransactionTypes)
            .NotNull()
            .WithMessage(@"Transaction types should not be null.");

        RuleFor(info => info.TransactionTypes)
            .Must(types => !types.Distinct().Any())
            .WithMessage("Transaction types contain duplicate items.")
            .When(info => info.TransactionTypes != null);

        RuleForEach(info => info.TransactionTypes)
            .Must(name =>
                name != null
                && (name.Equals(InventoryTransactionTypes.Issue, StringComparison.OrdinalIgnoreCase)
                    || name.Equals(InventoryTransactionTypes.Receive, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(@"Invalid transaction type name.");
    }
}
