using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

namespace InventoryManagement.Api.Features.Batches.DeleteBatchLine;

public class DeleteBatchLineEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapDelete(@"/api/v1/batch/", async () =>
        {

        })
            .RequireAuthorization(o => o.RequireRole(Roles.ScheduleManager))
            .WithBatchEndpointName(BatchEndpointNameConstants.DeleteBatchLine);
    }
}
