using FluentResults;

namespace InventoryManagement.Api.Errors;

public class UnauthorizedError : BaseError
{
    public override List<IError> Reasons => EmptyErrors;
    public override string Message => @"Does not have enough privileges to perform this action.";

    public static Result CreateFailureResultFromError()
    {
        return Result.Fail(new UnauthorizedError());
    }
    
    public static Result<TResult> CreateFailureResultFromError<TResult>()
    {
        return Result.Fail<TResult>(new UnauthorizedError());
    }
}