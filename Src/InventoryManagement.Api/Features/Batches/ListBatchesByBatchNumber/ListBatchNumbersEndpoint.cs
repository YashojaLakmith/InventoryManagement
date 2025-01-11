using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Batches.ListBatchesByBatchNumber;

public class ListBatchNumbersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(
            @"/api/v1/batch/",
            async (
                [FromQuery] string? inventoryItemId,
                [FromQuery] bool ignoreInactive,
                [FromQuery] int pageNumber,
                [FromQuery] int resultsPerPage,
                ISender sender) =>
            {
                BatchNumberFilter query = new(inventoryItemId, ignoreInactive, pageNumber, resultsPerPage);
                return await ListBatchesByBatchNumberAsync(sender, query);
            })
                .RequireAuthorization(policy => policy.RequireRole(Roles.SuperUser, Roles.ScheduleManager, Roles.Issuer, Roles.Receiver))
                .WithName(BatchEndpointNameConstants.ListBatchNumbers)
                .Produces<BatchNumberListResult>(StatusCodes.Status200OK)
                .Produces<List<IError>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> ListBatchesByBatchNumberAsync(ISender sender, BatchNumberFilter query)
    {
        Result<BatchNumberListResult> queryResult = await sender.Send(query);

        return queryResult.IsSuccess
            ? Results.Ok(queryResult.Value)
            : MatchErrors(queryResult);
    }

    private static IResult MatchErrors(ResultBase deleteResult)
    {
        return deleteResult.HasError<InvalidDataError>()
            ? Results.BadRequest(deleteResult.Errors)
            : Results.InternalServerError();
    }
}
