using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Users.RemoveRoles;

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleInformation, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ILogger<RemoveRoleCommandHandler> _logger;
    private readonly IValidator<RemoveRoleInformation> _validator;

    public RemoveRoleCommandHandler(
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        ILogger<RemoveRoleCommandHandler> logger,
        IValidator<RemoveRoleInformation> validator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result> Handle(RemoveRoleInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new NotImplementedException();
        }

        User? existingUser = await _userManager.FindByEmailAsync(request.EmailAddress);
        if (existingUser is null)
        {
            throw new NotImplementedException();
        }

        foreach (string role in request.RolesToRemove)
        {
            if (await _roleManager.RoleExistsAsync(role.ToUpper()))
            {
                throw new NotImplementedException();
            }
        }

        await _userManager.RemoveFromRolesAsync(existingUser, request.RolesToRemove.Select(role => role.ToUpper()));

        return Result.Ok();
    }
}
