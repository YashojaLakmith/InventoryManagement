using FluentValidation;

namespace InventoryManagement.Api.Features.Users.RemoveRoles;

public class RemoveRoleInformationValidator : AbstractValidator<RemoveRoleInformation>
{
    public RemoveRoleInformationValidator()
    {
        RuleFor(info => info.RolesToRemove.Distinct().Count() == info.RolesToRemove.Count)
            .Equal(true)
            .WithMessage(@"There are duplicates role names provided.");

        RuleFor(info => info.RolesToRemove.Count)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"At least one role should be provided");

        RuleFor(info => info.RolesToRemove.Count)
            .LessThanOrEqualTo(5)
            .WithMessage(@"Maximum roles can be removed at a time is 5");

        RuleFor(info => info.RolesToRemove.Contains(Roles.SuperUser, StringComparer.OrdinalIgnoreCase))
            .Equal(false)
            .WithMessage(@"Super user roles are non-modifiable.");

        RuleFor(info => info.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .WithMessage(@"Email address must be a valid one.");

        RuleForEach(info => info.RolesToRemove)
            .NotEmpty()
            .WithMessage(@"Role name cannot be empty.");

        RuleForEach(info => info.RolesToRemove)
            .Length(1, 10)
            .WithMessage(@"Role name should have maximum character length of 10.");
        
        RuleFor(info => info.RolesToRemove.Any(role => role.Contains(',')))
            .Equal(false)
            .WithMessage(@"Role name contains invalid characters.");
    }
}
