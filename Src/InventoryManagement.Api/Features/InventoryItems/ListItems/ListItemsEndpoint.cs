using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.InventoryItems.ListItems;

public class ListItemsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(
            "/api/v1/items",
            async (
                [FromQuery] int pageNumber,
                [FromQuery] int recordsPerPage,
                [FromQuery] string? partOfNameToSearch,
                ISender sender) =>
            {
                ListItemsQuery query = new(pageNumber, recordsPerPage, partOfNameToSearch);
                Result<ListItemsResult> queryResult = await sender.Send(query);

                return queryResult.IsSuccess
                    ? Results.Ok(queryResult.Value)
                    : MatchErrors(queryResult);
            })
                .RequireAuthorization(builder => builder.RequireRole(Roles.SuperUser, Roles.ScheduleManager, Roles.Issuer, Roles.Receiver))
                .WithName(InventoryItemEndpointNameConstants.ListItems)
                .Produces<ListItemsResult>(StatusCodes.Status200OK)
                .Produces<IError>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError);
    }

    private static IResult MatchErrors(ResultBase queryResult)
    {
        return queryResult.HasError<InvalidDataError>()
            ? Results.BadRequest(queryResult.Errors)
            : Results.InternalServerError();
    }
}
