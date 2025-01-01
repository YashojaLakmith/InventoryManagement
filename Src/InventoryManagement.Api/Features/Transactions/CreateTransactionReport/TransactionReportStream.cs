namespace InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

public record TransactionReportStream(Stream ReportDataStream, string ReportName, string FileExtension);