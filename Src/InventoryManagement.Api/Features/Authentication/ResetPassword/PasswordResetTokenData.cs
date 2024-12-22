using FluentResults;
using MediatR;

namespace InventoryManagement.Api.Features.Authentication.ResetPassword;

public record PasswordResetTokenData(
    string EmailAddress,
    string ResetToken,
    string NewPassword) : IRequest<Result>;