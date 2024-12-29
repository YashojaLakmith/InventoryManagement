using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Users.ListUsers;

public class ListUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(
            @"/api/v1/users",
            async (
                [FromQuery] int pageNumber,
                [FromQuery] int resultsPerPage,
                [FromQuery(Name = @"role")] string[]? rolesToFilter,
                ISender sender) =>
        {
            ListUserQuery query = new(resultsPerPage, pageNumber, rolesToFilter ?? []);
            return await ListUsersAsync(sender, query);
        })
            .RequireAuthorization(o => o.RequireRole(Roles.SuperUser, Roles.UserManager))
            .WithName(UserEndpointNameConstants.ListUsers)
            .Produces<IReadOnlyCollection<ListUserQueryResult>>(StatusCodes.Status200OK)
            .Produces<IError>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    public static async Task<IResult> ListUsersAsync(ISender sender, ListUserQuery query)
    {
        Result<ListUserQueryResult> queryResult = await sender.Send(query);

        return queryResult.IsSuccess
            ? Results.Ok(queryResult.Value)
            : MatchErrors(queryResult);
    }

    private static IResult MatchErrors(Result<ListUserQueryResult> queryResult)
    {
        return queryResult.HasError<InvalidDataError>()
                    ? Results.BadRequest(queryResult.Errors)
                    : Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
}