using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleInformationWithInvoker, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IValidator<AssignRoleInformation> _validator;
    private readonly ILogger<AssignRoleCommandHandler> _logger;

    public AssignRoleCommandHandler(
        UserManager<User> userManager,
        IValidator<AssignRoleInformation> validator,
        RoleManager<IdentityRole<int>> roleManager,
        ILogger<AssignRoleCommandHandler> logger)
    {
        _userManager = userManager;
        _validator = validator;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Result> Handle(AssignRoleInformationWithInvoker request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        User? invokingUser = await _userManager.FindByEmailAsync(request.InvokerEmailAddress);
        if (invokingUser is null)
        {
            return NotFoundError.CreateFailureResultFromError(@"Invoking user");
        }

        User? existingUser = await _userManager.FindByEmailAsync(request.EmailAddress);
        if (existingUser is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"User with email: {request.EmailAddress}");
        }

        bool isInvokerASuperUser = await _userManager.IsInRoleAsync(invokingUser, Roles.SuperUser);
        if (isInvokerASuperUser)
        {
            return await AssignRolesToExistingUserAsync(request.RolesToAssign, existingUser, cancellationToken);
        }

        if (request.RolesToAssign.Contains(Roles.SuperUser, StringComparer.OrdinalIgnoreCase) 
            || request.RolesToAssign.Contains(Roles.UserManager, StringComparer.OrdinalIgnoreCase))
        {
            return UnauthorizedError.CreateFailureResultFromError();
        }
        
        return await AssignRolesToExistingUserAsync(request.RolesToAssign, existingUser, cancellationToken);
    }

    private async Task<Result> AssignRolesToExistingUserAsync(
        IReadOnlyCollection<string> roles,
        User existingUser,
        CancellationToken cancellationToken)
    {
        HashSet<string> existingRoles = await _roleManager.Roles
            .AsNoTracking()
            .Select(r => r.Name!)
            .ToHashSetAsync(cancellationToken);

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
