
using FluentResults;

namespace InventoryManagement.Api.Errors;

public class NotFoundError : BaseError
{
    public NotFoundError(string resourceName)
    {
        Message = $@"Could not find {resourceName}.";
        Reasons = EmptyErrors;
    }
    
    public static Result CreateFailureResultFromError(string resourceName)
    {
        return Result.Fail(new NotFoundError(resourceName));
    }

    public static Result<T> CreateFailureResultFromError<T>(string resourceName)
    {
        return Result.Fail<T>(new NotFoundError(resourceName));
    }

    public override List<IError> Reasons { get; }
    public override string Message { get; }
}
