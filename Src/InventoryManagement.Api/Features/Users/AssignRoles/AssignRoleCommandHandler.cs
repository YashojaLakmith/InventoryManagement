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

    public async Task<Result> Handle(AssignRoleInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            IEnumerable<string> errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Fail(new InvalidDataError(errors));
        }

        User? existingUser = await _userManager.FindByEmailAsync(request.EmailAddress);
        if (existingUser is null)
        {
            return Result.Fail(new NotFoundError(@"User with given email address."));
        }

        HashSet<string?> existingRoles = await _roleManager.Roles
            .AsNoTracking()
            .Select(r => r.Name)
            .ToHashSetAsync(cancellationToken);

        string[] userProvidedRolesToUpperCase = request.RolesToAssign
            .Select(r => r.ToUpper())
            .ToArray();

        foreach (string roleName in userProvidedRolesToUpperCase)
        {
            if (!existingRoles.Contains(roleName))
            {
                return Result.Fail(new NotFoundError(roleName));
            }
        }

        await _userManager.AddToRolesAsync(existingUser, userProvidedRolesToUpperCase);
        return Result.Ok();
    }
}
