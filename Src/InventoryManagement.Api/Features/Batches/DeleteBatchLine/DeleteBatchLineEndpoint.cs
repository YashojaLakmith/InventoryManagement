using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Batches.DeleteBatchLine;

public class DeleteBatchLineEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapDelete(
            @"/api/v1/batch/", async (
            [FromQuery] string batchNumber,
            [FromQuery] string itemNumber,
            ISender sender) =>
        {
            DeleteBatchLineCommand command = new(batchNumber, itemNumber);
            return await DeleteBatchLineAsync(sender, command);
        })
            .RequireAuthorization(o => o.RequireRole(Roles.SuperUser, Roles.ScheduleManager))
            .WithName(BatchEndpointNameConstants.DeleteBatchLine)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest)
            .Produces<List<IError>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> DeleteBatchLineAsync(ISender sender, DeleteBatchLineCommand command)
    {
        Result deleteResult = await sender.Send(command);

        return deleteResult.IsSuccess
            ? Results.NoContent()
            : MatchErrors(deleteResult);
    }

    private static IResult MatchErrors(Result deleteResult)
    {
        if (deleteResult.HasError<InvalidDataError>())
        {
            return Results.BadRequest(deleteResult.Errors);
        }
        if (deleteResult.HasError<ActionNotAllowedError>())
        {
            return Results.BadRequest(deleteResult.Errors);
        }
        if (deleteResult.HasError<NotFoundError>())
        {
            return Results.BadRequest(deleteResult.Errors);
        }

        return Results.InternalServerError();
    }
}
