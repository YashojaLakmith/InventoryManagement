using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Authentication.Login;

public class LoginInformationValidator : AbstractValidator<LoginInformation>
{
    public LoginInformationValidator(IValidator<Email> emailValidator, IValidator<Password> passwordValidator)
    {
        RuleFor(info => new Email(info.EmailAddress))
            .SetValidator(emailValidator);

        RuleFor(info => new Password(info.Password))
            .SetValidator(passwordValidator);
    }
}
