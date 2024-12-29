using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Authentication.Errors;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Authentication.Login;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"/api/v1/auth/login", async (
            [FromBody] LoginInformation loginInfo,
            ISender sender) =>
        {
            return await LoginAsync(loginInfo, sender);
        })
            .AllowAnonymous()
            .WithName(AuthenticationEndpointNameConstants.LoginEndpoint)
            .Produces(StatusCodes.Status200OK)
            .Produces<List<IError>>(StatusCodes.Status404NotFound)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> LoginAsync(LoginInformation loginInfo, ISender sender)
    {
        Result loginResult = await sender.Send(loginInfo);

        return loginResult.IsSuccess
            ? Results.Ok()
            : MatchErrors(loginResult);
    }

    private static IResult MatchErrors(Result loginResult)
    {
        if (loginResult.HasError<NotFoundError>())
        {
            return Results.NotFound(loginResult.Errors);
        }

        if (loginResult.HasError<IncorrectPasswordError>())
        {
            return Results.BadRequest(loginResult.Errors);
        }

        if (loginResult.HasError<InvalidDataError>())
        {
            return Results.BadRequest(loginResult.Errors);
        }

        return Results.InternalServerError();
    }
}
