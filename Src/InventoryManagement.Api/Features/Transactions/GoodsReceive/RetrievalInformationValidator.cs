using FluentValidation;

namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public class RetrievalInformationValidator : AbstractValidator<RetrievalInformation>
{
    public RetrievalInformationValidator()
    {
        RuleFor(x => x.BatchNumber)
            .NotNull()
            .NotEmpty()
            .WithMessage(@"Batch number is required.");

        RuleFor(x => x.BatchNumber)
            .Length(1, 25)
            .WithMessage(@"Batch number should be no less than 1 character or more than 25 characters.");
        
        RuleFor(info => info.InventoryItemNumber)
            .NotEmpty()
            .WithMessage(@"Item Id cannot be empty");
        
        RuleFor(info => info.InventoryItemNumber)
            .Length(3, 25)
            .WithMessage(@"Item Id must be between 3 and 25 characters");
        
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
