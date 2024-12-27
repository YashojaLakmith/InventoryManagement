using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Api.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Users.ModifyPassword;

public class ModifyPasswordCommandHandler : IRequestHandler<ModifyPasswordInformationWithInvoker, Result>
{
    private readonly IValidator<ModifyPasswordInformation> _validator;
    private readonly ILogger<ModifyPasswordCommandHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public ModifyPasswordCommandHandler(
        IValidator<ModifyPasswordInformation> validator,
        ILogger<ModifyPasswordCommandHandler> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _validator = validator;
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result> Handle(ModifyPasswordInformationWithInvoker request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        User? existingUser = await _userManager.FindByEmailAsync(request.InvokerEmailAddress);
        if (existingUser is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"User with email: {request.InvokerEmailAddress}");
        }

        IdentityResult identityResult = await _userManager.ChangePasswordAsync(existingUser, request.CurrentPassword, request.NewPassword);
        if (identityResult.Succeeded)
        {
            await _signInManager.SignOutAsync();
            return Result.Ok();
        }
        
        InvalidDataError error = new([@"Current password is incorrect."]);
        return Result.Fail(error);
    }
}