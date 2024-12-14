
using FluentResults;

using InventoryManagement.Api.Features.InventoryItems.Errors;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.DeleteItem;

public class DeleteItemCommandHandler : IRequestHandler<ItemIdToDelete, Result>
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteItemCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(ItemIdToDelete request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        InventoryItem? existingItem = await _dbContext.InventoryItems.FindAsync(request.ItemId);
        if (existingItem is null)
        {
            return Result.Fail(new ItemNotFoundError());
        }

        _dbContext.Remove(existingItem);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
