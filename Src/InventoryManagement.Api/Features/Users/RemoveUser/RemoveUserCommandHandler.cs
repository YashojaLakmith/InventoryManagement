using System.Security.Claims;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Api.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Users.RemoveUser;

public class RemoveUserCommandHandler : IRequestHandler<RemoveUserInformation, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly ClaimsPrincipal _executingUser;
    private readonly ILogger<RemoveUserCommandHandler> _logger;
    private readonly IValidator<RemoveUserInformation> _validator;

    public RemoveUserCommandHandler(
        UserManager<User> userManager,
        ILogger<RemoveUserCommandHandler> logger,
        IValidator<RemoveUserInformation> validator,
        ClaimsPrincipal executingUser)
    {
        _userManager = userManager;
        _logger = logger;
        _validator = validator;
        _executingUser = executingUser;
    }

    public async Task<Result> Handle(RemoveUserInformation request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        User? existingUser = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (existingUser is null)
        {
            return NotFoundError.CreateFailureResultFromError($"User with id: {request.UserId}");
        }

        if (await _userManager.IsInRoleAsync(existingUser, Roles.SuperUser))
        {
            return ActionNotAllowedError.CreateFailureResultFromError(@"Cannot remove the Super User.");
        }

        User? executingUser = await GetExecutingUserAsync();
        if (executingUser is null)
        {
            return ConcurrencyViolationError.CreateFailureResultFromError(@"You no longer exist.");
        }

        if (await AreBothUsersUserManagers(existingUser, executingUser))
        {
            return UnauthorizedError.CreateFailureResultFromError();
        }

        await _userManager.DeleteAsync(existingUser);

        return Result.Ok();
    }

    private Task<User?> GetExecutingUserAsync()
    {
        string executingUserId = _executingUser.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return _userManager.FindByIdAsync(executingUserId);
    }

    private async Task<bool> AreBothUsersUserManagers(User existingUser, User executingUser)
    {
        return await _userManager.IsInRoleAsync(existingUser, Roles.UserManager)
               && await _userManager.IsInRoleAsync(executingUser, Roles.UserManager);
    }
}