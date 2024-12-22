using FluentResults;

namespace InventoryManagement.Api.Errors;

public class AlreadyExistsError : BaseError
{
    public AlreadyExistsError(string resourceName)
    {
        Message = $@"{resourceName} already exists.";
        Reasons = EmptyErrors;
    }

    public static Result CreateFailureResultFromError(string resourceName)
    {
        return Result.Fail(new AlreadyExistsError(resourceName));
    }

    public static Result<T> CreateFailureResultFromError<T>(string resourceName)
    {
        return Result.Fail<T>(new AlreadyExistsError(resourceName));
    }

    public override List<IError> Reasons { get; }
    public override string Message { get; }
}
