using FluentResults;
using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.InventoryItems.DeleteItem;

public class DeleteItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapDelete(@"/api/v1/items/{itemId:required}", async (
            [FromRoute] string itemId,
            ISender sender) =>
        {
            Result commandResult = await sender.Send(new ItemIdToDelete(itemId));

            return commandResult.IsSuccess 
                ? Results.NoContent() 
                : MatchErrors(commandResult);
        })
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<List<IError>>(StatusCodes.Status404NotFound)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static IResult MatchErrors(Result commandResult)
    {
        if (commandResult.HasError<NotFoundError>())
        {
            return Results.NotFound(commandResult.Errors);
        }
        else if (commandResult.HasError<InvalidDataError>())
        {
            return Results.BadRequest(commandResult.Errors);
        }

        return Results.InternalServerError();
    }
}
