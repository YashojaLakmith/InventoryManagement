
using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public class AssignRolesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPatch(@"api/v1/users/roles/", async (
            [FromBody] AssignRoleInformation assignRoleInformation,
            ISender sender) =>
        {
            Result result = await sender.Send(assignRoleInformation);

            return result.IsSuccess 
                ? Results.NoContent() 
                : MatchErrors(result);
        })
            .RequireAuthorization(o => o.RequireRole(Roles.UserManager, Roles.SuperUser))
            .Produces(StatusCodes.Status204NoContent)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest)
            .Produces<List<IError>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static IResult MatchErrors(Result result)
    {
        if (result.HasError<InvalidDataError>())
        {
            return Results.BadRequest(result.Errors);
        }
        else if (result.HasError<NotFoundError>())
        {
            return Results.NotFound(result.Errors);
        }

        return Results.InternalServerError();
    }
}
