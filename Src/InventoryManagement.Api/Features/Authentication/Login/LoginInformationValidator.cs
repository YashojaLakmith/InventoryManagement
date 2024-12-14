using FluentValidation;

namespace InventoryManagement.Api.Features.Authentication.Login;

public class LoginInformationValidator : AbstractValidator<LoginInformation>
{
    public LoginInformationValidator()
    {
        RuleFor(info => info.EmailAddress)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .WithMessage(@"Email address should be a valid email.");

        RuleFor(info => info.Password)
            .NotNull()
            .NotEmpty()
            .WithMessage(@"Password should not be empty.");

        RuleFor(info => info.Password.Length)
            .GreaterThanOrEqualTo(6)
            .LessThanOrEqualTo(14)
            .WithMessage(@"Minimum password length must be between 6 and 14 characters.");
    }
}
