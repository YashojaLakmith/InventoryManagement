using FluentValidation;

namespace InventoryManagement.Api.Features.InventoryItems.ViewItem;

public class ViewItemQueryValidator : AbstractValidator<ViewItemQuery>
{
    public ViewItemQueryValidator()
    {
        RuleFor(query => query.ItemId)
            .NotEmpty()
            .WithMessage("Item Id is required.")
            .Length(3, 25)
            .WithMessage("Item Id must be between 3 and 25 characters.");
    }
}
