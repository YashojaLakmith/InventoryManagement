using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public class RetrievalEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"api/v1/issue/", async () =>
        {

        })
            .RequireAuthorization(policy => policy.RequireRole([Roles.Receiver, Roles.ScheduleManager]));
    }
}
