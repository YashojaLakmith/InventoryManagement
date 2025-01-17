using FluentValidation;

namespace InventoryManagement.Api.Features.Shared.Validators;

public class EmailValidator : AbstractValidator<Email>
{
    private const string InvalidEmailMessage = @"Provided email is not valid.";

    public EmailValidator()
    {
        RuleFor(email => email.EmailAddress)
            .NotNull()
            .WithMessage(InvalidEmailMessage)
            .MaximumLength(256)
            .WithMessage(InvalidEmailMessage)
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            .WithMessage(InvalidEmailMessage);
    }
}

public readonly struct Email(string email)
{
    public readonly string EmailAddress = email;
}
