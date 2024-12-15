using FluentValidation;

namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public class NewBatchInformationValidator : AbstractValidator<NewBatchInformation>
{
    public NewBatchInformationValidator(IValidator<ItemOrder> validator)
    {
        RuleFor(x => x.UserEmail)
            .NotNull()
            .NotEmpty()
            .WithMessage(@"Email is required.");

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
    }
}
