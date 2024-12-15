using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.DeleteItem;

public record ItemIdToDelete(string ItemId) : IRequest<Result>;