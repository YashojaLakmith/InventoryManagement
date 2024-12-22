using FluentValidation;

namespace InventoryManagement.Api.Features.InventoryItems.CreateItem;

public class NewItemInformationValidator : AbstractValidator<NewItemInformation>
{
    public NewItemInformationValidator()
    {
        RuleFor(info => info.ItemId)
            .NotEmpty()
            .WithMessage("Item Id cannot be empty");
        
        RuleFor(info => info.ItemId)
            .Length(3, 25)
            .WithMessage("Item Id must be between 3 and 25 characters");
        
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
            .Length(1, 10)
            .WithMessage(@"Measurement unit must be between 1 and 10 characters");
    }
}
