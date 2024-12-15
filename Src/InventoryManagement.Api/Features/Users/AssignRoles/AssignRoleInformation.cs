using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public record AssignRoleInformation(
    string EmailAddress,
    IReadOnlyCollection<string> RolesToAssign
    ) : IRequest<Result>;