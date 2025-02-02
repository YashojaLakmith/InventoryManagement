using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.ListUsers;

public class ListUserQueryValidator : AbstractValidator<ListUserQuery>
{
    public ListUserQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0")
            .LessThan(int.MaxValue)
            .WithMessage($"Page number must be less than {int.MaxValue}");

        RuleFor(q => q.PageSize)
            .GreaterThanOrEqualTo(10)
            .WithMessage("Results per page must be between 10 and 25 items.")
            .LessThanOrEqualTo(25)
            .WithMessage("Results per page must be between 10 and 25 items.");

        RuleFor(info => info.Roles)
            .Must(roles => !AreThereDuplicateRoleNames(roles))
            .WithMessage("There are duplicates role names provided.")
            .Must(roles => roles.Count < 15)
            .WithMessage("Role count must be less than 16.")
            .ForEach(role => role.SetValidator(RoleNameValidator.Instance))
            .When(info => info.Roles.Any(r => r != null))
            .Must(roles => !roles.Any(r => r == null))
            .WithMessage("Role name should not be null");

        RuleFor(info => info.Roles)
            .Must(roles => roles.Count > 0)
            .WithMessage("At least one role should be provided");
    }

    private static bool AreThereDuplicateRoleNames(IReadOnlyCollection<string> roleNames)
        => roleNames.DistinctBy(role => role.ToLower()).Count() != roleNames.Count;
}