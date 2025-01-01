using InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

namespace InventoryManagement.Api.Infrastructure.Reports;

public static class ReportGeneratorExtensions
{
    public static void AddReportGenerators(this IServiceCollection services)
    {
        services.AddScoped<ITransactionReportGenerator, InventoryTransactionExcelReportGenerator>();
    }
}
