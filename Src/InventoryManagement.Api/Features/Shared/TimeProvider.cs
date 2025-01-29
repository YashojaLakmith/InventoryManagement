
using InventoryManagement.Api.Features.Shared.Abstractions;

namespace InventoryManagement.Api.Features.Shared;

internal class TimeProvider : ITimeProvider
{
    public DateTime CurrentUtcTime => DateTime.UtcNow;
}
