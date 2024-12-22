using FluentValidation;

namespace InventoryManagement.Api.Features.Authentication.RequestPasswordReset;

public class RequestPasswordResetQueryValidator : AbstractValidator<RequestPasswordResetQuery>
{
    public RequestPasswordResetQueryValidator()
    {
        RuleFor(info => info.EmailAddress)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .WithMessage(@"Email address should be a valid email.");
    }
}
