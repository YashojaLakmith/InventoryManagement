using FluentValidation;

namespace InventoryManagement.Api.Features.Users.ModifyPassword;

public class ModifyPasswordInformationValidator : AbstractValidator<ModifyPasswordInformation>
{
    public ModifyPasswordInformationValidator()
    {
        RuleFor(info => info.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required");
        
        RuleFor(info => info.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required");
        
        RuleFor(info => info.CurrentPassword.Equals(info.NewPassword))
            .Equal(false)
            .WithMessage(@"Current password and new password must be different");
        
        RuleFor(info => info.CurrentPassword)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z]).{7,15}$")
            .WithMessage(@"Password must be between 7 and 15 characters in length and must contain at least one upper case and lowercase letter.");
        
        RuleFor(info => info.NewPassword)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z]).{7,15}$")
            .WithMessage(@"Password must be between 7 and 15 characters in length and must contain at least one upper case and lowercase letter.");
    }
}