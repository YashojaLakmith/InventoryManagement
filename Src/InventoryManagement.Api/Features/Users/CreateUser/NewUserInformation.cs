using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Users.CreateUser;

public record NewUserInformation(
    string UserName,
    string EmailAddress,
    string Password
    ) : IRequest<Result<int>>;