using FluentResults;
using MediatR;

namespace InventoryManagement.Api.Features.Users.ViewUser;

public record UserIdQuery(int UserId) : IRequest<Result<UserView>>;