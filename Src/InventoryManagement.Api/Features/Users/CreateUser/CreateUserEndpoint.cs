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

            if (result.IsSuccess)
            {
                return Results.CreatedAtRoute(@"/api/v1/users/", result.Value);
            }
            else if (result.HasError<InvalidDataError>())
            {
                return Results.BadRequest(result.Errors);
            }
            else if (result.HasError<AlreadyExistsError>())
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.InternalServerError();
        })
            .RequireAuthorization(o => o.RequireRole(Roles.SuperUser, Roles.UserManager))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest);
    }
}
