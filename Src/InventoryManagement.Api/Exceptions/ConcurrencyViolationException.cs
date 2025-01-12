namespace InventoryManagement.Api.Exceptions;

public class ConcurrencyViolationException : Exception
{
    public ConcurrencyViolationException(Exception ex) : base(ex.Message, ex) { }

}
