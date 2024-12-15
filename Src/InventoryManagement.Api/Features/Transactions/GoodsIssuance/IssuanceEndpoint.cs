using FluentResults;

using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Transactions.GoodsIssuance;

public class IssuanceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"/api/v1/issue/", async (
            [FromBody] IssuanceInformation issuanceInfo,
            HttpContext httpContext,
            ISender sender) =>
        {
            Result transactionResult = await sender.Send(issuanceInfo);

            return Results.Created();
        })
            .RequireAuthorization(policy => policy.RequireRole([Roles.Issuer, Roles.ScheduleManager]));
    }
}
