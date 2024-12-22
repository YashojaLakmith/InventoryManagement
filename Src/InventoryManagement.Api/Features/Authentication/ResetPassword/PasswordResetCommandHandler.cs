using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Authentication.ResetPassword;

public class PasswordResetCommandHandler : IRequestHandler<PasswordResetTokenData, Result>
{
    private readonly IValidator<PasswordResetTokenData> _validator;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<PasswordResetTokenData> _logger;

    public PasswordResetCommandHandler(
        IValidator<PasswordResetTokenData> validator,
        UserManager<User> userManager,
        ILogger<PasswordResetTokenData> logger)
    {
        _validator = validator;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(PasswordResetTokenData request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            IEnumerable<string> errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
            return Result.Fail(new InvalidDataError(errorMessages));
        }

        User? existingUser = await _userManager.FindByEmailAsync(request.EmailAddress);
        if (existingUser is null)
        {
            return Result.Fail(new NotFoundError(@"User with the given email address"));
        }
        
        IdentityResult result = await _userManager.ResetPasswordAsync(existingUser, request.ResetToken, request.NewPassword);
        return !result.Succeeded 
            ? Result.Fail(new InvalidDataError([@"Reset token is invalid or expired."])) 
            : Result.Ok();
    }
}