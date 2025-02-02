using FluentValidation;

namespace InventoryManagement.Api.Features.Shared.Validators;

internal class RoleNameValidator : AbstractValidator<string?>
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
        RuleFor(roleName => roleName)
            .NotEmpty()
            .WithMessage(@"Role name cannot be empty.")
            .Matches(@"^(?=.{1,15}$)(?!.* {2})[A-Za-z]+(?: [A-Za-z]+)*$")
            .WithMessage(@"Incalid role name: Role name length should be less than 15 characters. Role name must only contain alphanumeric characters and spaces only. Spaces must not be placed at the begining or end of the role name or repeated.");
    }
}