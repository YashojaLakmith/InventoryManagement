using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Batches.ListBatchesByBatchNumber;

public class BatchNumberFilterValidator : AbstractValidator<BatchNumberFilter>
{
    public BatchNumberFilterValidator()
    {
        RuleFor(x => new InventoryItemNumber(x.InventoryItemId!))
            .SetValidator(InventoryItemNumberValidator.Instance)
            .When(x => x.InventoryItemId != null);

        RuleFor(x => x.IgnoreInactive)
            .NotNull()
            .WithMessage(@"Batch status is required.");

        RuleFor(q => q.PageNumber)
            .NotNull()
            .GreaterThan(0)
            .LessThan(int.MaxValue)
            .WithMessage(@"Page number must be greater than 0");

        RuleFor(q => q.ResultsPerPage)
            .NotNull()
            .GreaterThanOrEqualTo(10)
            .LessThanOrEqualTo(100)
            .WithMessage(@"Results per page must not be less than 10 and must not be greater than 100");
    }
}
