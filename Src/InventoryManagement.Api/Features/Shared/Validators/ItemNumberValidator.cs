using FluentValidation;

namespace InventoryManagement.Api.Features.Shared.Validators;

public class InventoryItemNumberValidator : AbstractValidator<InventoryItemNumber>
{
    private static InventoryItemNumberValidator? _instance;

    public static InventoryItemNumberValidator Instance
    {
        get
        {
            _instance ??= new InventoryItemNumberValidator();
            return _instance;
        }
    }

    private InventoryItemNumberValidator()
    {
        RuleFor(itemNumber => itemNumber.Value)
            .NotEmpty()
            .WithMessage(@"Item number is required.")
            .Matches(@"^(?!_)[A-Z0-9_]{1,25}(?<!_)$")
            .WithMessage(@"Item number length should be less than 25 characters. Item number must only contain uppercase letters, numbers and underscores and underscores must not be placed at the begining or end of the item number.");
    }
}

public readonly struct InventoryItemNumber(string inventoryItemNumber)
{
    public readonly string Value = inventoryItemNumber;
}