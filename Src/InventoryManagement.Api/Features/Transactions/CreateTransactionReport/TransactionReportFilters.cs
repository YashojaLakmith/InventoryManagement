using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

public class TransactionReportFilters() : IRequest<Result<Stream>>;