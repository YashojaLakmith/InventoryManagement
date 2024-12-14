using FluentResults;

namespace InventoryManagement.Api.Features.InventoryItems.Errors;

public record InvalidNewItemInformationError : IError
{
    private static readonly Dictionary<string, object> _emptyMetadata = [];
    private readonly List<IError> _errors;

    public InvalidNewItemInformationError(IEnumerable<string> errorMessages)
    {
        _errors = errorMessages
            .Select(e => (IError)new Error(e))
            .ToList();
    }

    public List<IError> Reasons => _errors;
    public string Message => @"Invalid information";
    public Dictionary<string, object> Metadata => _emptyMetadata;
}
