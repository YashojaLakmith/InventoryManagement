using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.CreateUser;

public class NewUserInformationValidator : AbstractValidator<NewUserInformation>
{
    public NewUserInformationValidator()
    {
        RuleFor(info => new Email(info.EmailAddress))
            .SetValidator(EmailValidator.Instance);

        RuleFor(info => info.UserName)
            .NotEmpty()
            .WithMessage(@"A user name is required.");

        RuleFor(info => info.UserName)
            .Length(3, 50)
            .WithMessage(@"User name length must be between 3 and 50 characters.");

        RuleFor(info => new Password(info.Password))
            .SetValidator(PasswordValidator.Instance);
    }
}
