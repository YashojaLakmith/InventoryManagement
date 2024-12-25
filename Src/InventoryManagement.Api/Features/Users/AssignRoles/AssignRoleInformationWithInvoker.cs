using FluentResults;
using MediatR;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public record AssignRoleInformationWithInvoker(
    string EmailAddress,
    string InvokerEmailAddress,
    IReadOnlyCollection<string> RolesToAssign) 
    : AssignRoleInformation(EmailAddress, RolesToAssign), IRequest<Result>;