using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Authentication.RequestPasswordReset;

public class RequestPasswordResetQueryValidator : AbstractValidator<RequestPasswordResetQuery>
{
    public RequestPasswordResetQueryValidator(IValidator<Email> emailValidator)
    {
        RuleFor(info => new Email(info.EmailAddress))
            .SetValidator(emailValidator);
    }
}
