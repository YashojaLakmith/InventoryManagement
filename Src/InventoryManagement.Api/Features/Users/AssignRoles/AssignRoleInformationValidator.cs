using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public class AssignRoleInformationValidator : AbstractValidator<AssignRoleInformation>
{
    public AssignRoleInformationValidator(IValidator<UserId> userIdValidator, IValidator<RoleName> roleNameValidator)
    {
        RuleFor(info => info.RolesToAssign.Distinct().Count() == info.RolesToAssign.Count)
            .Equal(true)
            .WithMessage(@"There are duplicates role names provided.");

        RuleFor(info => info.RolesToAssign.Count)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"At least one role should be provided");

        RuleFor(info => info.RolesToAssign.Contains(Roles.SuperUser, StringComparer.OrdinalIgnoreCase))
            .Equal(false)
            .WithMessage(@"Super user roles are non-modifiable.");

        RuleFor(info => info.RolesToAssign.Count)
            .LessThanOrEqualTo(5)
            .WithMessage(@"Maximum roles can be assigned at a time is 5");

        RuleForEach(info => info.RolesToAssign.Select(role => new RoleName(role)))
            .SetValidator(roleNameValidator);

        RuleFor(info => new UserId(info.UserId))
            .SetValidator(userIdValidator);
    }
}
