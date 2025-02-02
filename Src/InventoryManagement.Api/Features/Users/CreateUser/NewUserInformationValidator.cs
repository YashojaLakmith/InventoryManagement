using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.CreateUser;

public class NewUserInformationValidator : AbstractValidator<NewUserInformation>
{
    public NewUserInformationValidator()
    {
        RuleFor(info => new Email(info.EmailAddress))
            .SetValidator(EmailValidator.Instance)
            .When(info => info.EmailAddress != null)
            .Must(email => email.EmailAddress != null)
            .WithMessage("Email is required.");

        RuleFor(info => info.UserName)
            .Matches(@"^(?!.*([ .-])\1)(?=.{1,100}$)[A-Za-z][A-Za-z .-]*$")
            .WithMessage("Invalid user name.")
            .When(info => info.UserName != null)
            .NotNull()
            .WithMessage("A user name is required.");

        RuleFor(info => new Password(info.Password))
            .SetValidator(PasswordValidator.Instance)
            .When(info => info.Password != null)
            .Must(pw => pw.Value != null)
            .WithMessage("Password is required.");
    }
}
