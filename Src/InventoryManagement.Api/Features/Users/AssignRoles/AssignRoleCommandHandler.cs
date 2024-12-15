
using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.AspNetCore.Identity;

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

        ValidationResult validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new NotImplementedException();
        }

        User? existingUser = await _userManager.FindByEmailAsync(request.EmailAddress);
        if (existingUser is null)
        {
            throw new NotImplementedException();
        }

        foreach (string roleName in request.RolesToAssign)
        {
            if (!await _roleManager.RoleExistsAsync(roleName.ToUpper()))
            {
                throw new NotImplementedException();
            }
        }

        await _userManager.AddToRolesAsync(existingUser, request.RolesToAssign.Select(role => role.ToUpper()));
        return Result.Ok();
    }
}
