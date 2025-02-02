using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.InventoryItems.DeleteItem;

public class ItemIdToDeleteValidator : AbstractValidator<ItemIdToDelete>
{
    public ItemIdToDeleteValidator()
    {
        RuleFor(info => new InventoryItemNumber(info.ItemId))
            .SetValidator(InventoryItemNumberValidator.Instance);
    }
}
