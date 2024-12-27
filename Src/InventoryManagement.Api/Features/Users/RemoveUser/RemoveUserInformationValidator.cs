using FluentValidation;

namespace InventoryManagement.Api.Features.Users.RemoveUser;

public class RemoveUserInformationValidator : AbstractValidator<RemoveUserInformation>
{
    public RemoveUserInformationValidator()
    {
        RuleFor(info => info.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty");
        
        RuleFor(info => info.UserId)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Invalid UserId");
        
        RuleFor(info => info.UserId)
            .LessThanOrEqualTo(int.MaxValue)
            .WithMessage("Invalid UserId");
    }
}