using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public class ItemOrderValidator : AbstractValidator<ItemOrder>
{
    public ItemOrderValidator(IValidator<InventoryItemNumber> itemNumberValidator)
    {
        RuleFor(x => new InventoryItemNumber(x.ItemId))
            .SetValidator(itemNumberValidator);

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
