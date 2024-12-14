using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Users.CreateUser;

public record NewUserInformation() : IRequest<Result>;