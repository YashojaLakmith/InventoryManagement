using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Users.RemoveRoles;

public record RemoveRoleInformation(
    int UserId,
    IReadOnlyCollection<string> RolesToRemove
    ) : IRequest<Result>;