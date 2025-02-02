using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Transactions.GoodsIssuance;

public class IssuanceInformationValidator : AbstractValidator<IssuanceInformation>
{
    public IssuanceInformationValidator()
    {
        RuleFor(x => new BatchNumber(x.BatchNumber))
            .SetValidator(BatchNumberValidator.Instance);

        RuleFor(info => new InventoryItemNumber(info.ItemId))
            .SetValidator(InventoryItemNumberValidator.Instance);


        RuleFor(info => info.NumberOfItemsToIssue)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"Number of items to issue should not be less than 1")
            .LessThan(int.MaxValue)
            .WithMessage(@$"Number of items to issue should at most {int.MaxValue - 1}");
    }
}
