using FluentValidation;

namespace InventoryManagement.Api.Features.InventoryItems.DeleteItem;

public class ItemIdValidator : AbstractValidator<ItemIdToDelete>
{
    public ItemIdValidator()
    {
        RuleFor(info => info.ItemId)
            .NotEmpty()
            .WithMessage("Item Id cannot be empty");
        
        RuleFor(info => info.ItemId)
            .Length(3, 25)
            .WithMessage("Item Id must be between 3 and 25 characters");
    }
}
