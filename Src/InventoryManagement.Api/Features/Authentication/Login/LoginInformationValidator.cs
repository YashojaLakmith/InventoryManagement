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

        RuleFor(info => info.Password)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z]).{7,15}$")
            .WithMessage(@"Password must be between 7 and 15 characters in length and must contain at least one upper case and lowercase letter.");
    }
}
