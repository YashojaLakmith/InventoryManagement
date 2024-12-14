using FluentResults;

namespace InventoryManagement.Api.Features.Authentication.Errors;

public record InvalidLoginInformationError : IError
{
    private static readonly Dictionary<string, object> _emptyMetadata = [];
    private readonly List<IError> _errors;

    public InvalidLoginInformationError(IEnumerable<string> errors)
    {
        _errors = errors
            .Select(e => (IError)new Error(e))
            .ToList();
    }

    public List<IError> Reasons => _errors;
    public string Message => @"Login information is invalid.";
    public Dictionary<string, object> Metadata => _emptyMetadata;
}
