using FluentResults;

using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.InventoryItems.CreateItem;

public class CreateItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"/api/v1/items/", async (
            [FromBody] NewItemInformation itemInformation,
            ISender sender) =>
        {
            Result<string> commandResult = await sender.Send(itemInformation);

            return commandResult.IsSuccess
                ? Results.LocalRedirect(@$"/api/v1/items/{commandResult.Value}")
                : Results.BadRequest(commandResult.Errors);
        })
            .RequireAuthorization()
            .Produces<List<IResult>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status302Found)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}
