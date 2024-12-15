using FluentValidation;

namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public class ItemOrderValidator : AbstractValidator<ItemOrder>
{
    public ItemOrderValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty()
            .NotNull()
            .WithMessage(@"Item Id is required.");

        RuleFor(x => x.ItemId.Length)
            .LessThanOrEqualTo(25)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"Item id must have at least 1 character and less than 25 characters.");

        RuleFor(x => x.CostPerUnit)
            .NotEmpty()
            .WithMessage(@"Cost is required.");

        RuleFor(x => x.CostPerUnit)
            .GreaterThan(0)
            .WithMessage(@"Cost must be greater than 0");

        RuleFor(x => x.BatchSize)
            .NotEmpty()
            .WithMessage(@"Batch size is required.");

        RuleFor(x => x.BatchSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"Item count must be greater than 0");
    }
}
