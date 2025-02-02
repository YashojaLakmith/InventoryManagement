using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Authentication.RequestPasswordReset;

public class RequestPasswordResetQueryValidator : AbstractValidator<RequestPasswordResetQuery>
{
    public RequestPasswordResetQueryValidator()
    {
        RuleFor(info => new Email(info.EmailAddress))
            .SetValidator(EmailValidator.Instance);
    }
}
