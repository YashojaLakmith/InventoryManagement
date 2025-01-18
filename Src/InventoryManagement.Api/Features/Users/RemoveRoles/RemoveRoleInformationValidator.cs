using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.RemoveRoles;

public class RemoveRoleInformationValidator : AbstractValidator<RemoveRoleInformation>
{
    public RemoveRoleInformationValidator(IValidator<UserId> userIdValidator, IValidator<RoleName> roleNameValidator)
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

        RuleForEach(info => info.RolesToRemove.Select(role => new RoleName(role)))
            .SetValidator(roleNameValidator);

        RuleFor(info => new UserId(info.UserId))
            .SetValidator(userIdValidator);
    }
}
