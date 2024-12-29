using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

namespace InventoryManagement.Api.Features.Batches.CreateNewBatch;

public class CreateBatchEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"/api/v1/batch/", async () =>
        {

        })
            .RequireAuthorization(policy => policy.RequireRole([Roles.ScheduleManager]))
            .WithBatchEndpointName(BatchEndpointNameConstants.CreateBatch);
    }
}
