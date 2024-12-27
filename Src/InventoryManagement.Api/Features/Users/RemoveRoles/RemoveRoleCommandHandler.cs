using FluentResults;

using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Infrastructure.Database;
using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Users.RemoveRoles;

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleInformationWithInvoker, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ILogger<RemoveRoleCommandHandler> _logger;
    private readonly IValidator<RemoveRoleInformation> _validator;

    public RemoveRoleCommandHandler(
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        ILogger<RemoveRoleCommandHandler> logger,
        IValidator<RemoveRoleInformation> validator, 
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _validator = validator;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(RemoveRoleInformationWithInvoker request, CancellationToken cancellationToken)
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
            return NotFoundError.CreateFailureResultFromError($@"User with email: {request.EmailAddress}");
        }
        
        User? invokingUser = await _userManager.FindByEmailAsync(request.InvokerEmailAddress);
        if (invokingUser is null)
        {
            return NotFoundError.CreateFailureResultFromError($@"User with email: {request.InvokerEmailAddress}");
        }
        
        if (await _userManager.IsInRoleAsync(existingUser, Roles.SuperUser))
        {
            return UnauthorizedError.CreateFailureResultFromError();
        }
        
        if (await AreBothUsersUserManagersAsync(existingUser, invokingUser))
        {
            return UnauthorizedError.CreateFailureResultFromError();
        }

        HashSet<string> existingRoleNames = await GetExistingRoleNamesAsync(cancellationToken);
        string[] userProvidedRolesInUpperCase = ConvertRoleNamesToUppercase(request);

        foreach (string role in userProvidedRolesInUpperCase)
        {
            if (!existingRoleNames.Contains(role))
            {
                return NotFoundError.CreateFailureResultFromError($@"Role with name: {role}");
            }
        }

        await _userManager.RemoveFromRolesAsync(existingUser, userProvidedRolesInUpperCase);

        return Result.Ok();
    }

    private async Task<bool> AreBothUsersUserManagersAsync(User existingUser, User invokingUser)
    {
        return await _userManager.IsInRoleAsync(existingUser, Roles.UserManager)
               && await _userManager.IsInRoleAsync(invokingUser, Roles.UserManager);
    }
    
    private Task<HashSet<string>> GetExistingRoleNamesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Roles
            .AsNoTracking()
            .Select(role => role.Name!)
            .ToHashSetAsync(cancellationToken);
    }
    
    private static string[] ConvertRoleNamesToUppercase(RemoveRoleInformationWithInvoker request)
    {
        return request.RolesToRemove
            .Select(role => role.ToUpper())
            .ToArray();
    }
}