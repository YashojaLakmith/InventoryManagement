using FluentResults;
using MediatR;

namespace InventoryManagement.Api.Features.Users.RemoveRoles;

public record RemoveRoleInformationWithInvoker(
    string EmailAddress,
    string InvokerEmailAddress,
    IReadOnlyCollection<string> RolesToRemove)
    : RemoveRoleInformation(EmailAddress, RolesToRemove), IRequest<Result>;