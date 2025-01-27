using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.ListItems;

public record ListItemsQuery(
    int PageNumber,
    int RecordsPerPage,
    string? ItemNamePartToSearch) : IRequest<Result<ListItemsResult>>;