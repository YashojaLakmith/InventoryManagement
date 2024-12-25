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
        routeBuilder.MapGet(@"/api/v1/users", async (
                [FromQuery] int pageNumber,
                [FromQuery] int resultsPerPage,
                [FromQuery(Name = @"roles")] string[]? rolesToFilter,
                ISender sender) =>
            {
                ListUserQuery query = new(pageNumber, resultsPerPage, rolesToFilter ?? []);
                Result<ListUserQueryResult> queryResult = await sender.Send(query);

                if (queryResult.IsSuccess)
                {
                    return Results.Ok(queryResult.Value);
                }

                return queryResult.HasError<InvalidDataError>()
                    ? Results.BadRequest(queryResult.Errors)
                    : Results.StatusCode(StatusCodes.Status500InternalServerError);
            })
            .RequireAuthorization(o => o.RequireRole(Roles.SuperUser, Roles.UserManager))
            .Produces<IReadOnlyCollection<ListUserQueryResult>>(StatusCodes.Status200OK)
            .Produces<IError>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
} 