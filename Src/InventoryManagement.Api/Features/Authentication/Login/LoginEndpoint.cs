﻿using FluentResults;

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
            Result loginResult = await sender.Send(loginInfo);

            return loginResult.IsSuccess
                ? Results.Ok()
                : MatchErrors(loginResult);
        })
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .Produces<List<IError>>(StatusCodes.Status404NotFound)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static IResult MatchErrors(Result loginResult)
    {
        return loginResult.HasError<UserNotFoundError>()
            ? Results.NotFound(loginResult.Errors)
            : loginResult.HasError<IncorrectPasswordError>()
                ? Results.BadRequest(loginResult.Errors)
                : loginResult.HasError<InvalidLoginInformationError>()
                    ? Results.BadRequest(loginResult.Errors)
                    : Results.InternalServerError();
    }
}