using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.ModifyPassword;

public class ModifyPasswordInformationValidator : AbstractValidator<ModifyPasswordInformation>
{
    public ModifyPasswordInformationValidator(IValidator<Password> passwordValidator)
    {
        RuleFor(info => new Password(info.CurrentPassword))
            .SetValidator(passwordValidator);

        RuleFor(info => new Password(info.NewPassword))
            .SetValidator(passwordValidator);

        RuleFor(info => info.CurrentPassword.Equals(info.NewPassword))
            .Equal(false)
            .WithMessage(@"Current password and new password must be different");
    }
}