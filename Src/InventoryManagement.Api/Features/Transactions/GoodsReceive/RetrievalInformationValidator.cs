using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public class RetrievalInformationValidator : AbstractValidator<RetrievalInformation>
{
    public RetrievalInformationValidator(IValidator<BatchNumber> batchNumberValidator, IValidator<InventoryItemNumber> itemNumberValidator)
    {
        RuleFor(x => new BatchNumber(x.BatchNumber))
            .SetValidator(batchNumberValidator);

        RuleFor(info => new InventoryItemNumber(info.InventoryItemNumber))
            .SetValidator(itemNumberValidator);

        RuleFor(info => info.ItemCount)
            .NotEmpty()
            .WithMessage(@"Number of items to issue cannot be empty");

        RuleFor(info => info.ItemCount)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"Number of items to issue should at least be 1");

        RuleFor(info => info.ItemCount)
            .LessThanOrEqualTo(int.MaxValue)
            .WithMessage(@$"Number of items to issue should at most {int.MaxValue}");
    }
}
