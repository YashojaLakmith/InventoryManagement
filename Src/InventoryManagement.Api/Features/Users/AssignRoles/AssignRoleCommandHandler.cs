using System.Security.Claims;

using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleInformation, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly ClaimsPrincipal _executingUser;
    private readonly IUserRepository _userRespository;
    private readonly IValidator<AssignRoleInformation> _validator;
    private readonly ILogger<AssignRoleCommandHandler> _logger;

    public AssignRoleCommandHandler(
        UserManager<User> userManager,
        IValidator<AssignRoleInformation> validator,
        IUserRepository userRepository,
        ILogger<AssignRoleCommandHandler> logger,
        ClaimsPrincipal executingUser)
    {
        _userManager = userManager;
        _validator = validator;
        _userRespository = userRepository;
        _logger = logger;
        _executingUser = executingUser;
    }

    public async Task<Result> Handle(AssignRoleInformation request, CancellationToken cancellationToken)
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
            return NotFoundError.CreateFailureResultFromError(@"Invoking user");
        }

        User? existingUser = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (existingUser is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"User with Id: {request.UserId}");
        }

        bool isExecuterASuperUser = await _userManager.IsInRoleAsync(executingUser, Roles.SuperUser);

        return isExecuterASuperUser
            ? await AssignRolesToExistingUserAsync(request.RolesToAssign, existingUser, cancellationToken)
            : IsUserASuperUserOrUserManager(request)
            ? UnauthorizedError.CreateFailureResultFromError()
            : await AssignRolesToExistingUserAsync(request.RolesToAssign, existingUser, cancellationToken);
    }

    private Task<User?> GetExecutingUserAsync()
    {
        string executingUserEmail = _executingUser.FindFirstValue(ClaimTypes.Email)!;
        return _userManager.FindByEmailAsync(executingUserEmail);
    }

    private static bool IsUserASuperUserOrUserManager(AssignRoleInformation request)
    {
        return request.RolesToAssign.Contains(Roles.SuperUser, StringComparer.OrdinalIgnoreCase)
                || request.RolesToAssign.Contains(Roles.UserManager, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Result> AssignRolesToExistingUserAsync(
        IReadOnlyCollection<string> roles,
        User existingUser,
        CancellationToken cancellationToken)
    {
        HashSet<string> existingRoles = await _userRespository.GetAllRoleNamesAsync(cancellationToken);

        string[] userProvidedRolesToUpperCase = roles
            .Select(r => r.ToUpper())
            .ToArray();

        foreach (string roleName in userProvidedRolesToUpperCase)
        {
            if (!existingRoles.Contains(roleName))
            {
                return NotFoundError.CreateFailureResultFromError(roleName);
            }
        }

        await _userManager.AddToRolesAsync(existingUser, userProvidedRolesToUpperCase);
        return Result.Ok();
    }
}
