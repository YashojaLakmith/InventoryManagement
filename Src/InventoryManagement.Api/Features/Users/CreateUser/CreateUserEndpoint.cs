using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Users.CreateUser;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"/api/v1/users/", async (
            [FromBody] NewUserInformation newUserInformation,
            ISender sender) =>
        {
            Result<int> result = await sender.Send(newUserInformation);

            return result.IsSuccess
                ? Results.CreatedAtRoute(@"/api/v1/users/", result.Value)
                : MatchErrors(result);
        })
            .RequireAuthorization(o => o.RequireRole(Roles.SuperUser, Roles.UserManager))
            .WithName(UserEndpointNameConstants.CreateUser)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest);
    }

    private static IResult MatchErrors(Result<int> result)
    {
        if (result.HasError<InvalidDataError>())
        {
            return Results.BadRequest(result.Errors);
        }
        if (result.HasError<AlreadyExistsError>())
        {
            return Results.BadRequest(result.Errors);
        }

        return Results.InternalServerError();
    }
}
