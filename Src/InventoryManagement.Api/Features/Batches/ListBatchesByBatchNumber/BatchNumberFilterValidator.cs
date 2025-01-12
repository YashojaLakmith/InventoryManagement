using FluentValidation;

namespace InventoryManagement.Api.Features.Batches.ListBatchesByBatchNumber;

public class BatchNumberFilterValidator : AbstractValidator<BatchNumberFilter>
{
    public BatchNumberFilterValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .MinimumLength(1)
            .MaximumLength(25)
            .WithMessage(@"Item id must have at least 1 character and less than 25 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.InventoryItemId));

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
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage(@"Results per page must be greater than 1 and not greater than 100");
    }
}
