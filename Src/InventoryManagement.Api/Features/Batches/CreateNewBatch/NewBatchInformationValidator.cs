using FluentValidation;

namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public class NewBatchInformationValidator : AbstractValidator<NewBatchInformation>
{
    public NewBatchInformationValidator(IValidator<ItemOrder> orderValidator)
    {
        RuleFor(x => x.BatchNumber)
            .NotNull()
            .NotEmpty()
            .WithMessage(@"Batch number is required.");

        RuleFor(x => x.BatchNumber)
            .Length(1, 25)
            .WithMessage(@"Batch number should be no less than 1 character or more than 25 characters.");

        RuleFor(x => x.ItemOrders.Count)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"New batch should have at least one item.");

        RuleFor(x => x.ItemOrders.DistinctBy(o => o.ItemId).Count() == x.ItemOrders.Count())
            .Equal(true)
            .WithMessage(@"A single batch cannot have line item of same inventory item.");

        RuleForEach(x => x.ItemOrders)
            .SetValidator(orderValidator);
    }
}
