using FluentResults;
using InventoryManagement.Api.Errors;

namespace InventoryManagement.Api.Features.Transactions.TransactionErrors;

public class InsufficientQuantityError : BaseError
{
    public override List<IError> Reasons => EmptyErrors;
    public override string Message { get; }

    public InsufficientQuantityError(int requestedQuantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(requestedQuantity);
        
        Message = @$"Quantity is insufficient to issue {requestedQuantity} units.";
    }

    public static Result CreateFailureResultFromError(int requestedQuantity)
    {
        return Result.Fail(new InsufficientQuantityError(requestedQuantity));
    }
    
    public static Result<T> CreateFailureResultFromError<T>(int requestedQuantity)
    {
        return Result.Fail<T>(new InsufficientQuantityError(requestedQuantity));
    }
}