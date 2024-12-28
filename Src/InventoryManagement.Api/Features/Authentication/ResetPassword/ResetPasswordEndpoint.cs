using FluentResults;

using InventoryManagement.Api.Errors;
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
            return await ResetPasswordAsync(resetData, sender);
        })
            .AllowAnonymous()
            .WithName(AuthenticationEndpointNameConstants.ResetPassword)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest)
            .Produces<List<IError>>(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> ResetPasswordAsync(PasswordResetTokenData resetData, ISender sender)
    {
        Result resetResult = await sender.Send(resetData);

        return resetResult.IsSuccess
            ? Results.NoContent()
            : MatchErrors(resetResult);
    }

    private static IResult MatchErrors(Result resetResult)
    {
        if (resetResult.HasError<NotFoundError>())
        {
            return Results.NotFound(resetResult.Errors);
        }

        if (resetResult.HasError<InvalidDataError>())
        {
            return Results.BadRequest(resetResult.Errors);
        }

        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
}