using FluentResults;
using InventoryManagement.Api.Errors;

namespace InventoryManagement.Api.Features.Transactions.TransactionErrors;

public class SurplusQuantityError : BaseError
{
    private const string ErrorMessage = @"Trying to receive surplus quantity.";

    public override List<IError> Reasons => EmptyErrors;
    public override string Message => ErrorMessage;
    
    public SurplusQuantityError(){}

    public static Result CreateFailureResultFromError()
    {
        return Result.Fail(new SurplusQuantityError());
    }

    public static Result<T> CreateFailureResultFromError<T>()
    {
        return Result.Fail<T>(new SurplusQuantityError());
    }
}