using FluentResults;

using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public class CreateBatchEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"/api/v1/batch/", async (
            [FromBody] NewBatchInformation newBatchInformation,
            ISender sender) =>
        {
            return await CreateNewBatchAsync(newBatchInformation, sender);
        })
            .RequireAuthorization(policy => policy.RequireRole([Roles.ScheduleManager]))
            .WithBatchEndpointName(BatchEndpointNameConstants.CreateBatch);
    }

    public static async Task<IResult> CreateNewBatchAsync(NewBatchInformation newBatchInformation, ISender sender)
    {
        Result requestResult = await sender.Send(newBatchInformation);

        return requestResult.IsSuccess
            ? Results.Created()
            : MatchErrors(requestResult);
    }

    private static IResult MatchErrors(Result requestResult)
    {
        throw new NotImplementedException();
    }
}
