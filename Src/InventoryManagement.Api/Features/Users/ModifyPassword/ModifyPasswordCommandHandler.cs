using System.Security.Claims;

using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Users.ModifyPassword;

public class ModifyPasswordCommandHandler : IRequestHandler<ModifyPasswordInformation, Result>
{
    private readonly IValidator<ModifyPasswordInformation> _validator;
    private readonly ILogger<ModifyPasswordCommandHandler> _logger;
    private readonly ClaimsPrincipal _executingUser;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public ModifyPasswordCommandHandler(
        IValidator<ModifyPasswordInformation> validator,
        ILogger<ModifyPasswordCommandHandler> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ClaimsPrincipal executingUser)
    {
        _validator = validator;
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _executingUser = executingUser;
    }

    public async Task<Result> Handle(ModifyPasswordInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        User? executingUser = await GetExecutingUserAsync();
        return executingUser is null
            ? NotFoundError.CreateFailureResultFromError($@"You no longer exist.")
            : await TryChangePasswordAsync(request, executingUser);
    }

    private Task<User?> GetExecutingUserAsync()
    {
        string existingUserEmail = _executingUser.FindFirstValue(ClaimTypes.Email)!;
        return _userManager.FindByEmailAsync(existingUserEmail);
    }

    private async Task<Result> TryChangePasswordAsync(ModifyPasswordInformation request, User executingUser)
    {
        IdentityResult identityResult = await _userManager.ChangePasswordAsync(executingUser, request.CurrentPassword, request.NewPassword);
        if (identityResult.Succeeded)
        {
            await _signInManager.SignOutAsync();
            return Result.Ok();
        }

        InvalidDataError error = new([@"Current password is incorrect."]);
        return Result.Fail(error);
    }
}