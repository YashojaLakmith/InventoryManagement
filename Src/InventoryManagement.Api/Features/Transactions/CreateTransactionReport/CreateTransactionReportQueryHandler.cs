
using System.Data;

using ClosedXML.Excel;

using FluentResults;

using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

public class CreateTransactionReportQueryHandler : IRequestHandler<TransactionReportFilters, Result<Stream>>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateTransactionReportQueryHandler(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Stream>> Handle(TransactionReportFilters request, CancellationToken cancellationToken)
    {
        DataTable dataTable = new();
        dataTable.Columns.Add(@"Transaction ID", typeof(int));
        dataTable.Columns.Add(@"Transaction Date", typeof(DateOnly));
        dataTable.Columns.Add(@"Item ID", typeof(string));
        dataTable.Columns.Add(@"Item Name", typeof(string));
        dataTable.Columns.Add(@"Batch Number", typeof(string));
        dataTable.Columns.Add(@"Type", typeof(string));
        dataTable.Columns.Add(@"Units", typeof(int));
        dataTable.Columns.Add(@"Cost per Unit", typeof(decimal));
        dataTable.Columns.Add(@"Total cost", typeof(decimal));

        var transactionList = await _dbContext.TransactionRecords
            .Include(record => record.InventoryItem)
            .Include(record => record.BatchOfItem)
            .AsNoTracking()
            .OrderByDescending(record => record.Timestamp)
            .Select(record => new
            {
                record.RecordId,
                record.Timestamp,
                record.InventoryItem.InventoryItemId,
                record.InventoryItem.ItemName,
                record.BatchOfItem.BatchNumber,
                Type = record.TransactionUnitCount > 0 ? @"Received" : @"Issued",
                record.TransactionUnitCount,
                record.BatchOfItem.CostPerUnit,
                TotalCost = record.TransactionUnitCount * record.BatchOfItem.CostPerUnit
            })
            .ToListAsync(cancellationToken);

        foreach (var item in transactionList)
        {
            dataTable.Rows.Add(
                item.RecordId,
                item.Timestamp,
                item.ItemName,
                item.ItemName,
                item.BatchNumber,
                item.Type,
                item.TransactionUnitCount,
                item.CostPerUnit,
                item.TotalCost);
        }

        using XLWorkbook workbook = new();
        IXLWorksheet worksheet = workbook.AddWorksheet();
        worksheet.Cell(@"A1").SetValue(@"Transaction ID");
        worksheet.Cell(@"B1").SetValue(@"Transaction Date");
        worksheet.Cell(@"C1").SetValue(@"Item ID");
        worksheet.Cell(@"D1").SetValue(@"Item Name");
        worksheet.Cell(@"E1").SetValue(@"Batch Number");
        worksheet.Cell(@"F1").SetValue(@"Transaction Type");
        worksheet.Cell(@"G1").SetValue(@"Number of Units");
        worksheet.Cell(@"H1").SetValue(@"Cost per Unit");
        worksheet.Cell(@"I1").SetValue(@"Total Cost");

        worksheet.Cell(@"A2").InsertData(dataTable);
        MemoryStream memoryStream = new();
        workbook.SaveAs(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }
}
