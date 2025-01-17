using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.InventoryItems.ViewItem;

public class ViewItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(
            @"/api/v1/items/{itemId:required}",
            async (
                [FromRoute] string itemId,
                ISender sender) =>
            {
                ViewItemQuery query = new(itemId);
                Result<ItemDetails> queryResult = await sender.Send(query);

                return queryResult.IsSuccess
                    ? Results.Ok(queryResult.Value)
                    : MatchErrors(queryResult);
            })
                .RequireAuthorization(builder => builder.RequireRole(Roles.SuperUser, Roles.Issuer, Roles.Receiver, Roles.ScheduleManager))
                .WithName(InventoryItemEndpointNameConstants.ViewItem)
                .Produces<ItemDetails>(StatusCodes.Status200OK)
                .Produces<List<IError>>(StatusCodes.Status400BadRequest)
                .Produces<IError>(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError);
    }

    private static IResult MatchErrors(ResultBase queryResult)
    {
        if (queryResult.HasError<InvalidDataError>())
        {
            return Results.BadRequest(queryResult.Errors);
        }
        else if (queryResult.HasError<NotFoundError>())
        {
            return Results.NotFound(queryResult.Errors);
        }

        return Results.InternalServerError();
    }
}
