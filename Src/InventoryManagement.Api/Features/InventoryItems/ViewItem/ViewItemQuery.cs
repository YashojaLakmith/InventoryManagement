using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.ViewItem;

public record ViewItemQuery(string ItemId) : IRequest<Result<ItemDetails>>;