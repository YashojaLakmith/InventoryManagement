using FluentValidation;

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
        
        RuleForEach(q => q.Roles)
            .NotEmpty()
            .WithMessage(@"Role name cannot be empty");
        
        RuleFor(q => q.Roles.Count)
            .GreaterThan(0)
            .LessThan(15)
            .WithMessage(@"Role count must be greater than 0 and less than 15");
        
        RuleForEach(q => q.Roles)
            .NotEmpty()
            .WithMessage(@"Role name cannot be empty");

        RuleForEach(q => q.Roles)
            .Length(3, 10)
            .WithMessage(@"Role name length must be between 3 and 10 characters");
        
        RuleFor(q => q.Roles.Any(r => r.Contains(',')))
            .Equal(false)
            .WithMessage(@"Role contains invalid characters");
        
        RuleFor(q => q.Roles.Distinct().Count() == q.Roles.Count)
            .Equal(true)
            .WithMessage(@"There are duplicate roles");
    }
}