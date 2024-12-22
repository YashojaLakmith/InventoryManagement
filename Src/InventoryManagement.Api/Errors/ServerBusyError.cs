using FluentResults;

namespace InventoryManagement.Api.Errors;

public class ServerBusyError : BaseError
{
    private const string ErrorMessage = @"The server was unable to process the request.";
    
    public override List<IError> Reasons { get; }
    public override string Message => ErrorMessage;

    public ServerBusyError()
    {
        Reasons = EmptyErrors;
    }

    public static Result CreateFailureResultFromError()
    {
        return Result.Fail(new ServerBusyError());
    }
    
    public static Result<T> CreateFailureResultFromError<T>()
    {
        return Result.Fail<T>(new ServerBusyError());
    }
}