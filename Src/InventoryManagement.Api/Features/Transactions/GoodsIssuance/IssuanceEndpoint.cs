using FluentResults;

using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Transactions.GoodsIssuance;

public class IssuanceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"/api/v1/issue/", async (
            [FromBody] GoodsIssueDto issuanceInfo,
            HttpContext httpContext,
            ISender sender) =>
        {
            string? userEmail = IdentityUtlility.ExtractEmailFromClaims(httpContext.User);
            if (userEmail is null)
            {
                return Results.Unauthorized();
            }

            Result transactionResult = await sender.Send(
                new IssuanceInformation(issuanceInfo.ItemId, issuanceInfo.BatchNumber, issuanceInfo.NumberOfItemsToIssue, userEmail));

            return Results.Created();
        });
    }
}
