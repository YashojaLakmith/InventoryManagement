using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Authentication.RequestPasswordReset;

public class RequestPasswordResetQueryHandler : IRequestHandler<RequestPasswordResetQuery, Result>
{
    private readonly IValidator<RequestPasswordResetQuery> _validator;
    private readonly IEmailSender<User> _emailSender;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RequestPasswordResetQueryHandler> _logger;

    public RequestPasswordResetQueryHandler(
        IEmailSender<User> emailSender,
        UserManager<User> userManager,
        ILogger<RequestPasswordResetQueryHandler> logger,
        IValidator<RequestPasswordResetQuery> validator)
    {
        _emailSender = emailSender;
        _userManager = userManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result> Handle(RequestPasswordResetQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        User? existingUser = await _userManager.FindByEmailAsync(request.EmailAddress);
        if (existingUser is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"User with the email: {request.EmailAddress}");
        }

        await GenerateAndSendTokenAsync(existingUser);

        return Result.Ok();
    }

    private async Task GenerateAndSendTokenAsync(User user)
    {
        string passwordRestCode = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailSender.SendPasswordResetCodeAsync(user, user.Email!, passwordRestCode);
    }
}
