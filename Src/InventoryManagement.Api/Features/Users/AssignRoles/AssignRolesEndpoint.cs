using System.Security.Claims;
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
            ClaimsPrincipal user,
            ISender sender) =>
        {
            string invokerEmail = GetInvokerEmailAddress(user);
            AssignRoleInformationWithInvoker information = new(
                assignRoleInformation.EmailAddress,
                invokerEmail,
                assignRoleInformation.RolesToAssign);
            Result result = await sender.Send(information);

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

    private static string GetInvokerEmailAddress(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value!;
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
