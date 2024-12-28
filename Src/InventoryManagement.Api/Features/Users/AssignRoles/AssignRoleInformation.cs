using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public record AssignRoleInformation(
    int UserId,
    IReadOnlyCollection<string> RolesToAssign
    ) : IRequest<Result>;