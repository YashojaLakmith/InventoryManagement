namespace InventoryManagement.Api.Features.Shared.Abstractions;

public interface ITimeProvider
{
    DateTime CurrentUtcTime { get; }
}
