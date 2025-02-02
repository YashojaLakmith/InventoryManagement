using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.ModifyPassword;

public class ModifyPasswordInformationValidator : AbstractValidator<ModifyPasswordInformation>
{
    public ModifyPasswordInformationValidator()
    {
        RuleFor(info => new Password(info.NewPassword))
            .SetValidator(PasswordValidator.Instance)
            .When(info => info.NewPassword != null)
            .Must(pw => pw.Value != null)
            .WithMessage("New password is required.");

        RuleFor(info => new Password(info.CurrentPassword))
            .Must((info, currentPassword) => currentPassword.Value != info.NewPassword)
            .WithMessage("Current password and new password must be different.")
            .SetValidator(PasswordValidator.Instance)
            .When(info => info.CurrentPassword != null)
            .Must(pw => pw.Value != null)
            .WithMessage("Current password is required");
    }
}