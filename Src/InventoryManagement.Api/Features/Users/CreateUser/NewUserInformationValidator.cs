using FluentValidation;

namespace InventoryManagement.Api.Features.Users.CreateUser;

public class NewUserInformationValidator : AbstractValidator<NewUserInformation>
{
    public NewUserInformationValidator()
    {
        RuleFor(info => info.EmailAddress)
            .NotEmpty()
            .WithMessage(@"Email address is required.");

        RuleFor(info => info.EmailAddress)
            .EmailAddress()
            .WithMessage(@"Valid email address is required.");

        RuleFor(info => info.UserName)
            .NotEmpty()
            .WithMessage(@"A user name is required.");

        RuleFor(info => info.UserName)
            .Length(3, 50)
            .WithMessage(@"User name length must be between 3 and 50 characters.");

        RuleFor(info => info.Password)
            .NotEmpty()
            .WithMessage(@"Password is required.");
    }
}
