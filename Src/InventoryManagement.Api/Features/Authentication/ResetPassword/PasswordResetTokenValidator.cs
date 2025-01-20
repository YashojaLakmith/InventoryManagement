using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Authentication.ResetPassword;

public class PasswordResetTokenValidator : AbstractValidator<PasswordResetTokenData>
{
    public PasswordResetTokenValidator()
    {
        RuleFor(info => new Email(info.EmailAddress))
            .SetValidator(EmailValidator.Instance);

        RuleFor(info => new Password(info.NewPassword))
            .SetValidator(PasswordValidator.Instance);

        RuleFor(info => info.ResetToken)
            .NotEmpty()
            .WithMessage(@"Reset token is required.");
    }
}