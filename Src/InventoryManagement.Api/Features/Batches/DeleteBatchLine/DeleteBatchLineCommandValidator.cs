using FluentValidation;

namespace InventoryManagement.Api.Features.Batches.DeleteBatchLine;

public class DeleteBatchLineCommandValidator : AbstractValidator<DeleteBatchLineCommand>
{
    public DeleteBatchLineCommandValidator()
    {
        RuleFor(x => x.BatchNumber)
            .NotNull()
            .NotEmpty()
            .WithMessage(@"Batch number is required.");

        RuleFor(x => x.BatchNumber)
            .Length(1, 25)
            .WithMessage(@"Batch number should be no less than 1 character or more than 25 characters.");

        RuleFor(x => x.ItemNumber)
            .NotEmpty()
            .NotNull()
            .WithMessage(@"Item Id is required.");

        RuleFor(x => x.ItemNumber.Length)
            .LessThanOrEqualTo(25)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"Item id must have at least 1 character and less than 25 characters.");
    }
}
