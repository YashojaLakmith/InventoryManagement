using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.InventoryItems.CreateItem;

public class NewItemInformationValidator : AbstractValidator<NewItemInformation>
{
    public NewItemInformationValidator()
    {
        RuleFor(info => new InventoryItemNumber(info.ItemId))
            .SetValidator(InventoryItemNumberValidator.Instance);

        RuleFor(info => info.ItemName)
            .NotEmpty()
            .WithMessage("Item Name cannot be empty");

        RuleFor(info => info.ItemName)
            .Length(3, 50)
            .WithMessage("Item Name must be between 3 and 50 characters");

        RuleFor(info => info.MeasurementUnit)
            .NotEmpty()
            .WithMessage("Measurement unit cannot be empty");

        RuleFor(info => info.MeasurementUnit)
            .Length(1, 15)
            .WithMessage(@"Measurement unit must be between 1 and 10 characters");
    }
}
