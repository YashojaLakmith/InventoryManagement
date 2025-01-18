using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.InventoryItems.DeleteItem;

public class ItemIdToDeleteValidator : AbstractValidator<ItemIdToDelete>
{
    public ItemIdToDeleteValidator(IValidator<InventoryItemNumber> itemNumberValidator)
    {
        RuleFor(info => new InventoryItemNumber(info.ItemId))
            .SetValidator(itemNumberValidator);
    }
}
