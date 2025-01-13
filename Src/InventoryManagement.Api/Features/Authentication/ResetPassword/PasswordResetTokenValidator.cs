using FluentValidation;

namespace InventoryManagement.Api.Features.Authentication.ResetPassword;

public class PasswordResetTokenValidator : AbstractValidator<PasswordResetTokenData>
{
    public PasswordResetTokenValidator()
    {
        RuleFor(info => info.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .WithMessage(@"Email address should be a valid email.");

        RuleFor(info => info.NewPassword)
            .NotEmpty()
            .WithMessage(@"New password should not be empty.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z]).{7,15}$")
            .WithMessage(@"Password must be between 7 and 15 characters in length and must contain at least one upper case and lowercase letter.");

        RuleFor(info => info.ResetToken)
            .NotEmpty()
            .WithMessage(@"Reset token is required.");
    }
}