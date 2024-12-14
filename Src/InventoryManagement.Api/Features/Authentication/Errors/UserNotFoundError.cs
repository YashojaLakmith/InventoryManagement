
using FluentResults;

namespace InventoryManagement.Api.Features.Authentication.Errors;

public record UserNotFoundError : IError
{
    private static readonly List<IError> _emptyErrors = [];
    private static readonly Dictionary<string, object> _emptyMetadata = [];

    public List<IError> Reasons => _emptyErrors;
    public string Message => @"User not found";
    public Dictionary<string, object> Metadata => _emptyMetadata;
}
