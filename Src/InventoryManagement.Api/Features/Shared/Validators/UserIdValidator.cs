using FluentValidation;

namespace InventoryManagement.Api.Features.Shared.Validators;

public class UserIdValidator : AbstractValidator<UserId>
{
    public UserIdValidator()
    {
        RuleFor(info => info.Value)
            .NotNull()
            .WithMessage("UserId is required.")
            .LessThanOrEqualTo(int.MaxValue)
            .WithMessage(@"User Id value is too high.")
            .GreaterThanOrEqualTo(1)
            .WithMessage("User Id cannot be a zero or negative value.");
    }
}

public readonly struct UserId(int userId)
{
    public readonly int Value = userId;
}