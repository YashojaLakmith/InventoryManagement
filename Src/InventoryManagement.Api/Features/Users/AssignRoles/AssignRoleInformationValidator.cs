using FluentValidation;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public class AssignRoleInformationValidator : AbstractValidator<AssignRoleInformation>
{
    public AssignRoleInformationValidator()
    {
        RuleFor(info => AreThereDuplicates(info.RolesToAssign))
            .Equal(false)
            .WithMessage(@"There are duplicates role names provided.");

        RuleFor(info => info.RolesToAssign.Count)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"At least one role should be provided");

        RuleFor(info => info.RolesToAssign.Count)
            .LessThanOrEqualTo(5)
            .WithMessage(@"Maximum roles can be assigned is 5");

        RuleFor(info => info.EmailAddress)
            .EmailAddress()
            .WithMessage(@"Email address must be a valid one.");

        RuleForEach(info => info.RolesToAssign)
            .NotEmpty()
            .WithMessage(@"Role name cannot be empty.");

        RuleForEach(info => info.RolesToAssign)
            .Length(1, 10)
            .WithMessage(@"Role name should have maximum character length of 10.");
    }

    private static bool AreThereDuplicates(IReadOnlyCollection<string> roleNames)
    {
        return roleNames
            .Select(name => name.ToUpper())
            .Distinct()
            .Count() != roleNames.Count;
    }
}
