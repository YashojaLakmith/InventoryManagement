using FluentResults;

using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

public class CreateTransactionReportEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(@"/api/v1/reports/transaction/", async (
            [FromBody] TransactionReportFilters filters,
            ISender sender) =>
        {
            Result<Stream> fileStream = await sender.Send(filters);

            return Results.File(fileStream.Value, @"application/octet-stream",
                $@"Transaction Report: {DateTime.Now:D}.xlsx");
        });
        // .RequireAuthorization(policy => policy.RequireRole([
        //     Roles.ScheduleManager,
        //     Roles.ReportGenerator,
        //     Roles.Issuer,
        //     Roles.Receiver,
        //     Roles.SuperUser]));
    }
}
