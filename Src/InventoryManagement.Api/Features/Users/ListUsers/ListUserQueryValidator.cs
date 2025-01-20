using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.ListUsers;

public class ListUserQueryValidator : AbstractValidator<ListUserQuery>
{
    public ListUserQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage(@"Page number must be greater than 0");

        RuleFor(q => q.PageSize)
            .NotEmpty()
            .GreaterThan(0)
            .LessThan(15)
            .WithMessage(@"Page size must be greater than 1 and less than 15");

        RuleFor(q => q.Roles.Count)
            .GreaterThan(0)
            .LessThan(15)
            .WithMessage(@"Role count must be greater than 0 and less than 15");

        RuleForEach(q => q.Roles.Select(role => new RoleName(role)))
            .SetValidator(RoleNameValidator.Instance);

        RuleFor(q => q.Roles.Distinct().Count() == q.Roles.Count)
            .Equal(true)
            .WithMessage(@"There are duplicate roles");
    }
}