using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Authentication.Logout;

public class LogoutEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(
            @"/api/v1/auth/logout/",
            async (
                SignInManager<User> signInManager) =>
        {
            return await LogOutAsync(signInManager);
        })
            .RequireAuthorization()
            .WithName(AuthenticationEndpointNameConstants.LogoutEndpoint)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    public static async Task<IResult> LogOutAsync(SignInManager<User> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
}
