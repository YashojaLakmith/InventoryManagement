using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Authentication.Login;

public record LoginInformation(
    string EmailAddress,
    string Password)
    : IRequest<Result>;
