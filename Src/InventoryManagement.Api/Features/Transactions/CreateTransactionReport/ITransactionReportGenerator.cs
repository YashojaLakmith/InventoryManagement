namespace InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

public interface ITransactionReportGenerator
{
    Task<TransactionReportStream> GenerateInventoryTransactionReportAsync(
        TransactionReportFilters transactionReportFilters, CancellationToken cancellationToken = default);
}
