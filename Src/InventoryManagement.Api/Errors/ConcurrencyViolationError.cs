using FluentResults;

namespace InventoryManagement.Api.Errors;

public class ConcurrencyViolationError : BaseError
{
    public override List<IError> Reasons => EmptyErrors;
    public override string Message { get; }

    public ConcurrencyViolationError(string reason)
    {
        Message = reason;
    }

    public static Result CreateFailureResultFromError(string reason)
    {
        return Result.Fail(new ConcurrencyViolationError(reason));
    }
    
    public static Result<TResult> CreateFailureResultFromError<TResult>(string reason)
    {
        return Result.Fail<TResult>(new ConcurrencyViolationError(reason));
    }
}