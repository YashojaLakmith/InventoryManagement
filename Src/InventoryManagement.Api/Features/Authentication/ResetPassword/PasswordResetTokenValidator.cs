using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Authentication.ResetPassword;

public class PasswordResetTokenValidator : AbstractValidator<PasswordResetTokenData>
{
    public PasswordResetTokenValidator(IValidator<Email> emailValidator, IValidator<Password> passwordValidator)
    {
        RuleFor(info => new Email(info.EmailAddress))
            .SetValidator(emailValidator);

        RuleFor(info => new Password(info.NewPassword))
            .SetValidator(passwordValidator);

        RuleFor(info => info.ResetToken)
            .NotEmpty()
            .WithMessage(@"Reset token is required.");
    }
}