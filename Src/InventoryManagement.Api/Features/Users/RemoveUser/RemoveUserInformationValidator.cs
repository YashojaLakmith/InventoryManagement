using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Users.RemoveUser;

public class RemoveUserInformationValidator : AbstractValidator<RemoveUserInformation>
{
    public RemoveUserInformationValidator(IValidator<UserId> userIdValidator)
    {
        RuleFor(info => new UserId(info.UserId))
            .SetValidator(userIdValidator);
    }
}