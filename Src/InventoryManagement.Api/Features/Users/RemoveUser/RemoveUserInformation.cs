using FluentResults;
using MediatR;

namespace InventoryManagement.Api.Features.Users.RemoveUser;

public record RemoveUserInformation(
    int UserId) 
    : IRequest<Result>;