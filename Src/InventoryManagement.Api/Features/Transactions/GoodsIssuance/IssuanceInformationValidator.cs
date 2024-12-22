using FluentValidation;

namespace InventoryManagement.Api.Features.Transactions.GoodsIssuance;

public class IssuanceInformationValidator : AbstractValidator<IssuanceInformation>
{
    public IssuanceInformationValidator()
    {
        RuleFor(x => x.BatchNumber)
            .NotNull()
            .NotEmpty()
            .WithMessage(@"Batch number is required.");

        RuleFor(x => x.BatchNumber)
            .Length(1, 25)
            .WithMessage(@"Batch number should be no less than 1 character or more than 25 characters.");
        
        RuleFor(info => info.ItemId)
            .NotEmpty()
            .WithMessage(@"Item Id cannot be empty");
        
        RuleFor(info => info.ItemId)
            .Length(3, 25)
            .WithMessage(@"Item Id must be between 3 and 25 characters");
        
        RuleFor(info => info.NumberOfItemsToIssue)
            .NotEmpty()
            .WithMessage(@"Number of items to issue cannot be empty");

        RuleFor(info => info.NumberOfItemsToIssue)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"Number of items to issue should at least be 1");
        
        RuleFor(info => info.NumberOfItemsToIssue)
            .LessThanOrEqualTo(int.MaxValue)
            .WithMessage(@$"Number of items to issue should at most {int.MaxValue}");
    }
}
