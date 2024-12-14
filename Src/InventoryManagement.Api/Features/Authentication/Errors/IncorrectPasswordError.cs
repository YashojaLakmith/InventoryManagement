
using FluentResults;

namespace InventoryManagement.Api.Features.Authentication.Errors;

public sealed record IncorrectPasswordError : IError
{
    private static readonly Dictionary<string, object> _metadata = [];
    private static readonly List<IError> _errors = [];

    public List<IError> Reasons => _errors;
    public string Message => @"Password is incorrect.";
    public Dictionary<string, object> Metadata => _metadata;
}
