using FluentResults;

using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.InventoryItems.DeleteItem;

public class DeleteItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapDelete(@"/api/v1/items/{itemId:guid}", async (
            [FromRoute] Guid itemId,
            ISender sender) =>
        {
            Result commandResult = await sender.Send(new ItemIdToDelete(itemId));

            return commandResult.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(commandResult.Errors);
        })
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<List<IError>>(StatusCodes.Status404NotFound)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}
