using FluentResults;
using MediatR;

namespace InventoryManagement.Api.Features.Authentication.RequestPasswordReset;

public record RequestPasswordResetQuery(
    string EmailAddress) : IRequest<Result>;
