using FluentResults;
using MediatR;

namespace InventoryManagement.Api.Features.Users.ModifyPassword;

public record ModifyPasswordInformationWithInvoker(
    string CurrentPassword,
    string NewPassword,
    string InvokerEmailAddress)
    : ModifyPasswordInformation(CurrentPassword, NewPassword),
        IRequest<Result>;