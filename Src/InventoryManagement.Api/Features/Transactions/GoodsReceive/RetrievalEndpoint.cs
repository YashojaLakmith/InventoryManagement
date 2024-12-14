using System.Security.Claims;

using FluentResults;

using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public class RetrievalEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"/api/v1/retrieve/", async (
            [FromBody] RetrievalDto retrievalInfo,
            HttpContext httpContext,
            ISender sender) =>
        {
            Claim? email = httpContext.User.FindFirst(ClaimTypes.Email);
            if (email is null)
            {
                return Results.Unauthorized();
            }

            Result result = await sender.Send(
                new RetrievalInformation(retrievalInfo.ItemId, retrievalInfo.BatchNumber, retrievalInfo.CountToRetrieve, email.Value));

            return Results.NoContent();
        });
    }
}
