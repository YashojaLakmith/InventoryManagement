using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Users.RemoveUser;

public class RemoveUserEndpoint : IEndpoint
{
    public const string EndnpointName = @"Remove User";

    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapDelete(@"/api/v1/users/{userId:int}", async (
            [FromRoute] int userId,
            ISender sender) =>
        {
            RemoveUserInformation request = new(userId);
            Result requestResult = await sender.Send(request);

            return requestResult.IsSuccess
                ? Results.NoContent()
                : MatchErrors(requestResult);
        })
        .WithName(EndnpointName)
        .RequireAuthorization(o => o.RequireRole(Roles.UserManager, Roles.SuperUser))
        .WithName(UserEndpointNameConstants.DeleteUser)
        .Produces<List<IError>>(StatusCodes.Status409Conflict)
        .Produces<List<IError>>(StatusCodes.Status400BadRequest)
        .Produces<List<IError>>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status500InternalServerError);
    }

    private static IResult MatchErrors(Result requestResult)
    {
        if (requestResult.HasError<InvalidDataError>())
        {
            return Results.BadRequest(requestResult.Errors);
        }

        if (requestResult.HasError<NotFoundError>())
        {
            return Results.NotFound(requestResult.Errors);
        }

        if (requestResult.HasError<UnauthorizedError>())
        {
            return Results.Forbid();
        }

        if (requestResult.HasError<ActionNotAllowedError>())
        {
            return Results.BadRequest(requestResult.Errors);
        }

        if (requestResult.HasError<ConcurrencyViolationError>())
        {
            return Results.Conflict(requestResult.Errors);
        }

        return Results.InternalServerError();
    }
}