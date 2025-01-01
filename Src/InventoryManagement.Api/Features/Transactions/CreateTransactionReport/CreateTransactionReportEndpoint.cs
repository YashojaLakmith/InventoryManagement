using FluentResults;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Utilities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

public class CreateTransactionReportEndpoint : IEndpoint
{
    private const string ContentType = @"application/octet-stream";

    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(
            @"/api/v1/reports/inventory-transaction/",
            async (
                [FromQuery] DateTime? sinceDate,
                [FromQuery] DateTime? toDate,
                [FromQuery(Name = @"transaction_type")] string[]? transactionTypes,
                ISender sender) =>
        {
            TransactionReportFilters filter = new(
                sinceDate ?? DateTime.MinValue,
                toDate ?? DateTime.UtcNow,
                transactionTypes ?? [InventoryTransactionTypes.Issue, InventoryTransactionTypes.Receive]);

            return await GenerateReportAsync(filter, sender);
        })
         .RequireAuthorization(policy => policy.RequireRole(
             Roles.ScheduleManager,
             Roles.ReportGenerator,
             Roles.Issuer,
             Roles.Receiver,
             Roles.SuperUser))
         .WithName(TransactionRecordEndpointNameConstants.CreateTransactionReport)
         .Produces(StatusCodes.Status200OK, contentType: ContentType)
         .Produces<List<IError>>(StatusCodes.Status400BadRequest)
         .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GenerateReportAsync(TransactionReportFilters filters, ISender sender)
    {
        Result<TransactionReportStream> fileStreamResult = await sender.Send(filters);

        return fileStreamResult.IsSuccess
            ? CreateFileResult(fileStreamResult.Value)
            : MatchErrors(fileStreamResult);
    }

    private static IResult CreateFileResult(TransactionReportStream reportStream)
    {
        reportStream.ReportDataStream.Position = 0;
        return Results.File(
            reportStream.ReportDataStream,
            ContentType,
            $@"{reportStream.ReportName}{reportStream.FileExtension}");
    }

    private static IResult MatchErrors(Result<TransactionReportStream> reportGenerationResult)
    {
        return reportGenerationResult.HasError<InvalidDataError>()
            ? Results.BadRequest(reportGenerationResult.Errors)
            : Results.InternalServerError();
    }
}
