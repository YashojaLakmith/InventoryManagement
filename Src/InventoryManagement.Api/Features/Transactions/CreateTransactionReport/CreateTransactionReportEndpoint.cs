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
        routeBuilder.MapPost(@"/api/v1/reports/inventory-transaction/", async (
            [FromBody] TransactionReportFilters filters,
            ISender sender) =>
        {
            return await GenerateReportAsync(filters, sender);
        })
         .RequireAuthorization(policy => policy.RequireRole(
             Roles.ScheduleManager,
             Roles.ReportGenerator,
             Roles.Issuer,
             Roles.Receiver,
             Roles.SuperUser))
         .WithName(TransactionRecordEndpointNameConstants.CreateTransactionReport);
    }

    public static async Task<IResult> GenerateReportAsync(TransactionReportFilters filters, ISender sender)
    {
        Result<Stream> fileStreamResult = await sender.Send(filters);

        return fileStreamResult.IsSuccess
            ? CreateFileResult(fileStreamResult.Value)
            : MatchErrors(fileStreamResult);
    }

    private static IResult CreateFileResult(Stream fileStream)
    {
        fileStream.Position = 0;
        return Results.File(
            fileStream,
            @"application/octet-stream",
            $@"Inventory Transaction Report: {DateTime.Now:D}.xlsx");
    }

    private static IResult MatchErrors(Result<Stream> reportGenerationResult)
    {
        throw new NotImplementedException();
    }
}
