using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public class RetrievalInformationValidator : AbstractValidator<RetrievalInformation>
{
    public RetrievalInformationValidator()
    {
        RuleFor(x => new BatchNumber(x.BatchNumber))
            .SetValidator(BatchNumberValidator.Instance);

        RuleFor(info => new InventoryItemNumber(info.InventoryItemNumber))
            .SetValidator(InventoryItemNumberValidator.Instance);

        RuleFor(info => info.ItemCount)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"Number of items to issue should not be less than 1")
            .LessThan(int.MaxValue)
            .WithMessage(@$"Number of items to issue should at most {int.MaxValue - 1}");
    }
}
