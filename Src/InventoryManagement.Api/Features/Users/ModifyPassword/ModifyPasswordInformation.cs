using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Users.ModifyPassword;

public record ModifyPasswordInformation(string CurrentPassword, string NewPassword) : IRequest<Result>;