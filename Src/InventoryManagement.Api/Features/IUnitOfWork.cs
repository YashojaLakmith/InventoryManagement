namespace InventoryManagement.Api.Features;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void ClearChanges();
}
