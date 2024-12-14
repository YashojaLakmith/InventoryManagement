
using FluentResults;

namespace InventoryManagement.Api.Features.InventoryItems.Errors;

public class ItemNotFoundError : IError
{
    public List<IError> Reasons { get; }
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }
}
