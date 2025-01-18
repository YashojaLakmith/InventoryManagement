using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.InventoryItems.ViewItem;

public class ViewItemQueryValidator : AbstractValidator<ViewItemQuery>
{
    public ViewItemQueryValidator(IValidator<InventoryItemNumber> itemNumberValidator)
    {
        RuleFor(query => new InventoryItemNumber(query.ItemId))
            .SetValidator(itemNumberValidator);
    }
}
