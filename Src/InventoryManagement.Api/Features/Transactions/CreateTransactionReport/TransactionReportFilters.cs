using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

public record TransactionReportFilters(
    DateTime SinceDate,
    DateTime ToDate,
    string[] TransactionTypes) : IRequest<Result<TransactionReportStream>>;