﻿using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Users.AssignRoles;

public class AssignRolesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPatch(
            @"api/v1/users/{userId:int}/assign-roles",
            async (
                [FromRoute] int userId,
                [FromQuery(Name = @"rolename")] string[] rolesToAssign,
                ISender sender) =>
        {
            AssignRoleInformation request = new(userId, rolesToAssign);
            return await AssignRolesAsync(sender, request);
        })
            .RequireAuthorization(o => o.RequireRole(Roles.UserManager, Roles.SuperUser))
            .WithName(UserEndpointNameConstants.AssignRoles)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<List<IError>>(StatusCodes.Status400BadRequest)
            .Produces<List<IError>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> AssignRolesAsync(ISender sender, AssignRoleInformation request)
    {
        Result requestResult = await sender.Send(request);

        return requestResult.IsSuccess
            ? Results.NoContent()
            : MatchErrors(requestResult);
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
        if (result.HasError<UnauthorizedError>())
        {
            return Results.Forbid();
        }

        return Results.InternalServerError();
    }
}
