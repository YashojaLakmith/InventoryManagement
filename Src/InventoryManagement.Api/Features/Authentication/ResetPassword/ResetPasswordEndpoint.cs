using FluentResults;
using InventoryManagement.Api.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Authentication.ResetPassword;

public class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPatch(@"/api/v1/auth/reset-password", async (
                [FromBody] PasswordResetTokenData resetData,
                ISender sender) =>
            {
                Result resetResult = await sender.Send(resetData);

                return Results.NoContent();
            })
            .AllowAnonymous();
    }
}