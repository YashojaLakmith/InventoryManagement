using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Users.RemoveRoles;

public record RemoveRoleInformation(
    string EmailAddress,
    IReadOnlyCollection<string> RolesToRemove
    );