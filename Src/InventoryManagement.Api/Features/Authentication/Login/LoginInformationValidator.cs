using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Authentication.Login;

public class LoginInformationValidator : AbstractValidator<LoginInformation>
{
    public LoginInformationValidator()
    {
        RuleFor(info => new Email(info.EmailAddress))
            .SetValidator(EmailValidator.Instance);

        RuleFor(info => new Password(info.Password))
            .SetValidator(PasswordValidator.Instance);
    }
}
