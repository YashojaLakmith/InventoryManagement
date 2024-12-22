
using FluentResults;
using FluentValidation.Results;

namespace InventoryManagement.Api.Errors;

public class InvalidDataError : BaseError
{
    private static readonly Dictionary<string, object> EmptyMetadata = [];

    public InvalidDataError()
    {
        Reasons = [];
    }

    public InvalidDataError(IEnumerable<string> reasons)
    {
        IEnumerable<Error> errors = reasons.Select(reason => new Error(reason));
        Reasons = [.. errors];
    }

    public static Result CreateFailureResultFromError(IEnumerable<ValidationFailure> validationFailures)
    {
        IEnumerable<string> errorMessages = validationFailures.Select(failure => failure.ErrorMessage);
        return Result.Fail(new InvalidDataError(errorMessages));
    }
    
    public static Result<T> CreateFailureResultFromError<T>(IEnumerable<ValidationFailure> validationFailures)
    {
        IEnumerable<string> errorMessages = validationFailures.Select(failure => failure.ErrorMessage);
        return Result.Fail<T>(new InvalidDataError(errorMessages));
    }

    public override List<IError> Reasons { get; }
    public override string Message => @"Provided data is invalid.";
}
