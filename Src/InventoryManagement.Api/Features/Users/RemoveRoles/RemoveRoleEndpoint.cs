using System.Security.Claims;
using FluentResults;
using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Users.RemoveRoles;

public class RemoveRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPatch(@"/api/v1/users/remove-roles/", async (
            [FromBody] RemoveRoleInformation removeRoleInformation,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            string invokerEmail = user.FindFirst(ClaimTypes.Email)!.Value;
            RemoveRoleInformationWithInvoker request = new(
                removeRoleInformation.EmailAddress,
                invokerEmail,
                removeRoleInformation.RolesToRemove);
            
            Result requestResult = await sender.Send(request);
            
            return requestResult.IsSuccess
                ? Results.NoContent()
                : MatchErrors(requestResult);
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
        if (result.HasError<NotFoundError>())
        {
            return Results.NotFound(result.Errors);
        }
        if (result.HasError<UnauthorizedError>())
        {
            return Results.Forbid();
        }

        return Results.InternalServerError();
    }
}
