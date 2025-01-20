using FluentValidation;

namespace InventoryManagement.Api.Features.Shared.Validators;

public class BatchNumberValidator : AbstractValidator<BatchNumber>
{
    private static BatchNumberValidator? _instance;

    public static BatchNumberValidator Instance
    {
        get
        {
            _instance ??= new BatchNumberValidator();
            return _instance;
        }
    }

    private BatchNumberValidator()
    {
        RuleFor(batchNumber => batchNumber.Value)
            .NotEmpty()
            .WithMessage(@"Batch number is required.")
            .Matches(@"^(?!_)[A-Z0-9_]{1,25}(?<!_)$")
            .WithMessage(@"Batch number length should be less than 25 characters. Batch number must only contain uppercase letters, numbers and underscores and underscores must not be placed at the begining or end of the batch number.");
    }
}

public readonly struct BatchNumber(string batchNumber)
{
    public readonly string Value = batchNumber;
}