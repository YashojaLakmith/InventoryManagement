
using FluentResults;

namespace InventoryManagement.Api.Errors;

public abstract class BaseError : IError
{
    private static readonly Dictionary<string, object> EmptyMetadata = [];
    protected static readonly List<IError> EmptyErrors = [];

    public abstract List<IError> Reasons { get; }
    public abstract string Message { get; }
    public Dictionary<string, object> Metadata => EmptyMetadata;
}
