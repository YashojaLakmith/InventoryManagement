using FluentValidation;

namespace InventoryManagement.Api.Features.Shared.Validators;

internal class UserIdValidator : AbstractValidator<UserId>
{
    private static UserIdValidator? _instance;

    public static UserIdValidator Instance
    {
        get
        {
            _instance ??= new UserIdValidator();
            return _instance;
        }
    }

    private UserIdValidator()
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