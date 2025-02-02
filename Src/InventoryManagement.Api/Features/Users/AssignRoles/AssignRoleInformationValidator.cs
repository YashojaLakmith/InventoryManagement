using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public class AssignRoleInformationValidator : AbstractValidator<AssignRoleInformation>
{
    public AssignRoleInformationValidator()
    {
        RuleFor(info => info.RolesToAssign)
            .Must(roles => !AreThereDuplicateRoleNames(roles))
            .WithMessage("There are duplicates role names provided.")
            .Must(roles => !roles.Any(r => r.Equals(Roles.SuperUser, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Super user roles are non-modifiable.")
            .Must(roles => roles.Count < 6)
            .WithMessage("Maximum roles can be assigned at a time is 5")
            .ForEach(role => role.SetValidator(RoleNameValidator.Instance))
            .When(info => info.RolesToAssign.Any(r => r != null))
            .Must(roles => !roles.Any(r => r == null))
            .WithMessage("Role name should not be null");

        RuleFor(info => info.RolesToAssign)
            .Must(roles => roles.Count > 0)
            .WithMessage("At least one role should be provided");

        RuleFor(info => new UserId(info.UserId))
            .SetValidator(UserIdValidator.Instance);
    }

    private static bool AreThereDuplicateRoleNames(IReadOnlyCollection<string> roleNames)
        => roleNames.DistinctBy(role => role.ToLower()).Count() != roleNames.Count;
}
