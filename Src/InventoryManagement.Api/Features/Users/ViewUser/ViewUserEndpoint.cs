using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Users.ViewUser;

public class ViewUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(@"/api/v1/users/{userId:int}", async (
            [FromRoute] int userId,
            ISender sender) =>
        {
            UserIdQuery query = new(userId);
            Result<UserView> queryResult = await sender.Send(query);

            return queryResult.IsSuccess
                ? Results.Ok(queryResult.Value)
                : MatchErrors(queryResult);
        })
        .RequireAuthorization(o => o.RequireRole(Roles.UserManager, Roles.SuperUser))
        .WithName(UserEndpointNameConstants.ViewUser)
        .Produces<UserView>(StatusCodes.Status200OK)
        .Produces<List<IError>>(StatusCodes.Status400BadRequest)
        .Produces<List<IError>>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status500InternalServerError);
    }

    private static IResult MatchErrors(Result<UserView> queryResult)
    {
        if (queryResult.HasError<InvalidDataError>())
        {
            return Results.BadRequest(queryResult.Errors);
        }

        if (queryResult.HasError<NotFoundError>())
        {
            return Results.NotFound(queryResult.Errors);
        }

        return Results.InternalServerError();
    }
}