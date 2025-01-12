using System.Security.Claims;

using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Users.RemoveRoles;

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleInformation, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly ClaimsPrincipal _executingUser;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RemoveRoleCommandHandler> _logger;
    private readonly IValidator<RemoveRoleInformation> _validator;

    public RemoveRoleCommandHandler(
        UserManager<User> userManager,
        ILogger<RemoveRoleCommandHandler> logger,
        IValidator<RemoveRoleInformation> validator,
        IUserRepository userRepository,
        ClaimsPrincipal executingUser)
    {
        _userManager = userManager;
        _logger = logger;
        _validator = validator;
        _userRepository = userRepository;
        _executingUser = executingUser;
    }

    public async Task<Result> Handle(RemoveRoleInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        User? executingUser = await GetExecutingUserAsync();
        if (executingUser is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"You");
        }

        User? targetUser = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (targetUser is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"User with email: {request.UserId}");
        }

        if (await _userManager.IsInRoleAsync(targetUser, Roles.SuperUser))
        {
            return UnauthorizedError.CreateFailureResultFromError();
        }

        if (await AreBothUsersUserManagersAsync(executingUser, targetUser))
        {
            return UnauthorizedError.CreateFailureResultFromError();
        }

        HashSet<string> existingRoleNames = await _userRepository.GetAllRoleNamesAsync(cancellationToken);
        string[] userProvidedRolesInUpperCase = ConvertRoleNamesToUppercase(request);

        foreach (string role in userProvidedRolesInUpperCase)
        {
            if (!existingRoleNames.Contains(role))
            {
                return NotFoundError.CreateFailureResultFromError($@"Role with name: {role}");
            }
        }

        await _userManager.RemoveFromRolesAsync(targetUser, userProvidedRolesInUpperCase);

        return Result.Ok();
    }

    private Task<User?> GetExecutingUserAsync()
    {
        string executingUserEmail = _executingUser.FindFirstValue(ClaimTypes.Email)!;
        return _userManager.FindByEmailAsync(executingUserEmail);
    }

    private async Task<bool> AreBothUsersUserManagersAsync(User executingUser, User targetUser)
    {
        return await _userManager.IsInRoleAsync(executingUser, Roles.UserManager)
               && await _userManager.IsInRoleAsync(targetUser, Roles.UserManager);
    }

    private static string[] ConvertRoleNamesToUppercase(RemoveRoleInformation request)
    {
        return request.RolesToRemove
            .Select(role => role.ToUpper())
            .ToArray();
    }
}