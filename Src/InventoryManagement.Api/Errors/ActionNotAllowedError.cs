using FluentResults;

namespace InventoryManagement.Api.Errors;

public class ActionNotAllowedError : BaseError
{
    public override List<IError> Reasons { get; }
    public override string Message => @"Action is not allowed.";

    public ActionNotAllowedError(string reason)
    {
        Reasons = [new Error(reason)];
    }

    public static Result CreateFailureResultFromError(string reason)
    {
        return Result.Fail(new ActionNotAllowedError(reason));   
    }
    
    public static Result<TResult> CreateFailureResultFromError<TResult>(string reason)
    {
        return Result.Fail<TResult>(new ActionNotAllowedError(reason));   
    }
}