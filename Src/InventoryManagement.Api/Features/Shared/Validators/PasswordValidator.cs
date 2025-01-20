using FluentValidation;

namespace InventoryManagement.Api.Features.Shared.Validators;

public class PasswordValidator : AbstractValidator<Password>
{
    private static PasswordValidator? _instance;

    public static PasswordValidator Instance
    {
        get
        {
            _instance ??= new PasswordValidator();
            return _instance;
        }
    }

    private PasswordValidator()
    {
        RuleFor(password => password.Value)
            .NotEmpty()
            .WithMessage(@"Password cannot be null or empty.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z]).{7,15}$")
            .WithMessage(@"Password must be between 7 and 15 characters in length and must contain at least one upper case and lowercase letter.");
    }
}

public readonly struct Password(string password)
{
    public readonly string Value = password;
}