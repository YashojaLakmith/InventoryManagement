using FluentValidation;

namespace InventoryManagement.Api.Features.Shared.Validators;

public class RoleNameValidator : AbstractValidator<RoleName>
{
    private static RoleNameValidator? _instance;

    public static RoleNameValidator Instance
    {
        get
        {
            _instance ??= new RoleNameValidator();
            return _instance;
        }
    }

    private RoleNameValidator()
    {
        RuleFor(roleName => roleName.Value)
            .NotEmpty()
            .WithMessage(@"Role name cannot be null or empty.")
            .Matches(@"^(?!_)[A-Za-z0-9_]{1,15}(?<!_)$")
            .WithMessage(@"Incalid role name: Role name length should be less than 15 characters. Role name must only contain alphanumeric characters and underscores only. Underscores must not be placed at the begining or end of the role name.");
    }
}

public readonly struct RoleName(string roleName)
{
    public readonly string Value = roleName;
}