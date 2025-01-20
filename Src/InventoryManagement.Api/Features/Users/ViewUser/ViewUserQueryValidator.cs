using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.ViewUser;

public class ViewUserQueryValidator : AbstractValidator<UserIdQuery>
{
    public ViewUserQueryValidator()
    {
        RuleFor(info => new UserId(info.UserId))
            .SetValidator(UserIdValidator.Instance);
    }
}