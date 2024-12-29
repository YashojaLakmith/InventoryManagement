using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Users.ModifyPassword;

public class ModifyPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPatch(
            @"/api/v1/users/modify-password",
            async (
                [FromBody] ModifyPasswordInformation passwordInformation,
                ISender sender) =>
        {
            return await ModifyPasswordAsync(passwordInformation, sender);
        })
        .RequireAuthorization()
        .WithName(UserEndpointNameConstants.ModifyPassword)
        .Produces(StatusCodes.Status204NoContent)
        .Produces<List<IError>>(StatusCodes.Status400BadRequest)
        .Produces<List<IError>>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> ModifyPasswordAsync(ModifyPasswordInformation passwordInformation, ISender sender)
    {
        Result modificationResult = await sender.Send(passwordInformation);

        return modificationResult.IsSuccess
            ? Results.NoContent()
            : MatchErrors(modificationResult);
    }

    private static IResult MatchErrors(Result result)
    {
        if (result.HasError<InvalidDataError>())
        {
            return Results.BadRequest(result.Errors);
        }
        if (result.HasError<NotFoundError>())
        {
            return Results.NotFound(result.Errors);
        }

        return Results.InternalServerError();
    }
}