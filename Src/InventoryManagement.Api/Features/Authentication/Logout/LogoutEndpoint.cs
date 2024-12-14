using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Authentication.Logout;

public class LogoutEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(@"/api/v1/auth/logout/", async (SignInManager<User> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        })
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}
