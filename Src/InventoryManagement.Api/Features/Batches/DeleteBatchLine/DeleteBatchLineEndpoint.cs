using FluentResults;

using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Batches.DeleteBatchLine;

public class DeleteBatchLineEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapDelete(@"/api/v1/batch/", async (
            [FromQuery] string batchNumber,
            [FromQuery] string itemNumber,
            ISender sender) =>
        {
            DeleteBatchLineCommand command = new(batchNumber, itemNumber);
            return await DeleteBatchLineAsync(sender, command);
        })
            .RequireAuthorization(o => o.RequireRole(Roles.ScheduleManager))
            .WithBatchEndpointName(BatchEndpointNameConstants.DeleteBatchLine);
    }

    public static async Task<IResult> DeleteBatchLineAsync(ISender sender, DeleteBatchLineCommand command)
    {
        Result deleteResult = await sender.Send(command);

        return deleteResult.IsSuccess
            ? Results.NoContent()
            : MatchErrors(deleteResult);
    }

    private static IResult MatchErrors(Result deleteResult)
    {
        throw new NotImplementedException();
    }
}
