using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.InventoryItems.CreateItem;

public class CreateItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"/api/v1/items/", async (
            [FromBody] NewItemInformation itemInformation,
            ISender sender) =>
        {
            return await CreateNewItemAsync(itemInformation, sender);
        })
            .RequireAuthorization(policy => policy.RequireRole(Roles.SuperUser, Roles.ScheduleManager))
            .WithName(InventoryItemEndpointNameConstants.CreateItem)
            .Produces<List<IResult>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    public static async Task<IResult> CreateNewItemAsync(NewItemInformation itemInformation, ISender sender)
    {
        Result<string> commandResult = await sender.Send(itemInformation);

        return commandResult.IsSuccess
            ? Results.Created(@"/api/v1/items/", commandResult.Value)
            : MatchErrors(commandResult);
    }

    private static IResult MatchErrors(Result<string> commandResult)
    {
        if (commandResult.HasError<InvalidDataError>())
        {
            return Results.BadRequest(commandResult.Errors);
        }
        else if (commandResult.HasError<AlreadyExistsError>())
        {
            return Results.BadRequest(commandResult.Errors);
        }

        return Results.InternalServerError();
    }
}
