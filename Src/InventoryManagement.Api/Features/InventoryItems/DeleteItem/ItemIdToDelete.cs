using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.DeleteItem;

public record ItemIdToDelete(Guid ItemId) : IRequest<Result>;