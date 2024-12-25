using FluentValidation;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public class AssignRoleInformationValidator : AbstractValidator<AssignRoleInformation>
{
    public AssignRoleInformationValidator()
    {
        RuleFor(info => info.RolesToAssign.Distinct().Count() == info.RolesToAssign.Count)
            .Equal(true)
            .WithMessage(@"There are duplicates role names provided.");

        RuleFor(info => info.RolesToAssign.Count)
            .GreaterThanOrEqualTo(1)
            .WithMessage(@"At least one role should be provided");

        RuleFor(info => info.RolesToAssign.Count)
            .LessThanOrEqualTo(5)
            .WithMessage(@"Maximum roles can be assigned at a time is 5");

        RuleFor(info => info.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .WithMessage(@"Email address must be a valid one.");

        RuleForEach(info => info.RolesToAssign)
            .NotEmpty()
            .WithMessage(@"Role name cannot be empty.");

        RuleForEach(info => info.RolesToAssign)
            .Length(1, 10)
            .WithMessage(@"Role name should have maximum character length of 10.");
        
        RuleFor(info => info.RolesToAssign.Any(role => role.Contains(',')))
            .Equal(false)
            .WithMessage(@"Role name contains invalid characters.");
    }
}
