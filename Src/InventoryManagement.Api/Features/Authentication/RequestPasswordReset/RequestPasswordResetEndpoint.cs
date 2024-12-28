using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Authentication.RequestPasswordReset;

public class RequestPasswordResetEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"api/v1/auth/request-password-reset/", async (
                [FromBody] RequestPasswordResetQuery query,
                ISender sender) =>
        {
            return await RequestForPasswordResetAsync(query, sender);
        })
            .AllowAnonymous()
            .WithName(AuthenticationEndpointNameConstants.RequestResetPassword)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest)
            .Produces<List<IError>>(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> RequestForPasswordResetAsync(RequestPasswordResetQuery query, ISender sender)
    {
        Result queryResult = await sender.Send(query);

        return queryResult.IsSuccess
            ? Results.Ok()
            : MatchErrors(queryResult);
    }

    private static IResult MatchErrors(Result queryResult)
    {
        if (queryResult.HasError<NotFoundError>())
        {
            return Results.NotFound(queryResult.Errors);
        }

        if (queryResult.HasError<InvalidDataError>())
        {
            return Results.BadRequest(queryResult.Errors);
        }

        return Results.InternalServerError();
    }
}
