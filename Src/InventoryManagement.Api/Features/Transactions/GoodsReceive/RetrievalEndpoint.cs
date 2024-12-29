using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Transactions.TransactionErrors;
using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Transactions.GoodsReceive;

public class RetrievalEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"api/v1/receive/", async (
                [FromBody] RetrievalInformation retrievalInformation,
                ISender sender) =>
        {
            return await ReceiveGoodsAsync(retrievalInformation, sender);
        })
            .RequireAuthorization(policy => policy.RequireRole(Roles.Receiver, Roles.ScheduleManager))
            .WithName(TransactionRecordEndpointNameConstants.ReceiveGoods)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest)
            .Produces<List<IError>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden); ;
    }

    public static async Task<IResult> ReceiveGoodsAsync(RetrievalInformation retrievalInformation, ISender sender)
    {
        Result transactionResult = await sender.Send(retrievalInformation);

        return transactionResult.IsSuccess
            ? Results.Created()
            : MatchErrors(transactionResult);
    }

    private static IResult MatchErrors(Result transactionResult)
    {
        if (transactionResult.HasError<InvalidDataError>())
        {
            return Results.BadRequest(transactionResult.Errors);
        }
        if (transactionResult.HasError<NotFoundError>())
        {
            return Results.NotFound(transactionResult.Errors);
        }
        if (transactionResult.HasError<SurplusQuantityError>())
        {
            return Results.BadRequest(transactionResult.Errors);
        }
        if (transactionResult.HasError<ServerBusyError>())
        {
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
}
