using InventoryManagement.Api.Utilities;

namespace InventoryManagement.Api.Features.Users.CreateUser;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"api/v1/users/", async () =>
        {
            return Results.Created();
        });
    }
}
