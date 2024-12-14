using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.CreateItem;

public record NewItemInformation(
    string ItemId,
    string ItemName,
    string MeasurementUnit
    ) : IRequest<Result<string>>;