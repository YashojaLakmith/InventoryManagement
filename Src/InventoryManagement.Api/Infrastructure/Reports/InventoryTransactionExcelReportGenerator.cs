using System.Data;

using ClosedXML.Excel;

using InventoryManagement.Api.Features.Transactions;
using InventoryManagement.Api.Features.Transactions.CreateTransactionReport;
using InventoryManagement.Api.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Infrastructure.Reports;

public class InventoryTransactionExcelReportGenerator : ITransactionReportGenerator
{
    private readonly ApplicationDbContext _dbContext;

    public InventoryTransactionExcelReportGenerator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TransactionReportStream> GenerateInventoryTransactionReportAsync(
        TransactionReportFilters transactionReportFilters, CancellationToken cancellationToken = default)
    {
        DataTable dataTable = CreateDataTable();
        IQueryable<TransactionRecord> query = CreateInitialQuery();
        query = ApplyDateRangeFilter(query, transactionReportFilters.SinceDate, transactionReportFilters.ToDate);
        query = ApplyTransactionTypeFilter(query, transactionReportFilters.TransactionTypes);
        await FetchDataToDataTableAsync(dataTable, query, cancellationToken);
        MemoryStream memoryStream = CreateWorkBook(dataTable);

        return new TransactionReportStream(memoryStream, $@"Inventory Transaction Report_{DateTime.Now}", @".xlsx");
    }

    private static DataTable CreateDataTable()
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

        return dataTable;
    }

    private IQueryable<TransactionRecord> CreateInitialQuery() =>
            _dbContext.TransactionRecords
                .Include(record => record.InventoryItem)
                .Include(record => record.BatchOfItem)
                .AsNoTracking();

    private static IQueryable<TransactionRecord> ApplyTransactionTypeFilter(IQueryable<TransactionRecord> dataSource, string[] transactionTypes)
    {
        if (transactionTypes.Contains(InventoryTransactionTypes.Issue, StringComparer.OrdinalIgnoreCase))
        {
            dataSource = dataSource.Where(record => record.TransactionUnitCount < 0);
        }

        if (transactionTypes.Contains(InventoryTransactionTypes.Receive, StringComparer.OrdinalIgnoreCase))
        {
            dataSource = dataSource.Where(record => record.TransactionUnitCount > 0);
        }

        return dataSource;
    }

    private static IQueryable<TransactionRecord> ApplyDateRangeFilter(IQueryable<TransactionRecord> dataSource, DateTime fromDate, DateTime toDate)
    {
        return dataSource.Where(record => record.Timestamp >= fromDate && record.Timestamp <= toDate);
    }

    private static async Task FetchDataToDataTableAsync(DataTable dataTable, IQueryable<TransactionRecord> query, CancellationToken cancellationToken)
    {
        var transactionList = await query
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

        foreach (
            var item in transactionList)
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
    }

    private static MemoryStream CreateWorkBook(DataTable dataTable)
    {
        using XLWorkbook workbook = new();
        IXLWorksheet worksheet = CreateWorksheet(workbook);
        worksheet.Cell(@"A2").InsertData(dataTable);
        return SaveWorkBook(workbook);
    }

    private static IXLWorksheet CreateWorksheet(XLWorkbook workbook)
    {
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
        return worksheet;
    }

    private static MemoryStream SaveWorkBook(XLWorkbook workbook)
    {
        MemoryStream memoryStream = new();
        workbook.SaveAs(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }
}
