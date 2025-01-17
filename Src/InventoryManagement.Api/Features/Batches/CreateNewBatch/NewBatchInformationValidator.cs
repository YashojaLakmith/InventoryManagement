using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public class NewBatchInformationValidator : AbstractValidator<NewBatchInformation>
{
    public NewBatchInformationValidator(IValidator<ItemOrder> orderValidator, IValidator<BatchNumber> batchNumberValidator)
    {
        RuleFor(x => new BatchNumber(x.BatchNumber))
            .SetValidator(batchNumberValidator);

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
