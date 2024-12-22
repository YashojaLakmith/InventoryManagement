using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.InventoryItems.DeleteItem;

public class DeleteItemCommandHandler : IRequestHandler<ItemIdToDelete, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IValidator<ItemIdToDelete> _validator;
    private readonly ILogger<DeleteItemCommandHandler> _logger;

    public DeleteItemCommandHandler(
        ApplicationDbContext dbContext,
        IValidator<ItemIdToDelete> validator,
        ILogger<DeleteItemCommandHandler> logger)
    {
        _dbContext = dbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> Handle(ItemIdToDelete request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            IEnumerable<string> errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
            return Result.Fail(new InvalidDataError(errorMessages));
        }

        InventoryItem? existingItem = await _dbContext.InventoryItems
            .FirstOrDefaultAsync(item => item.InventoryItemId == request.ItemId, cancellationToken);
        if (existingItem is null)
        {
            return Result.Fail(new NotFoundError(@$"Inventory item with id: {request.ItemId}"));
        }

        await RemoveFromDatabaseAsync(existingItem, cancellationToken);

        return Result.Ok();
    }

    private Task<int> RemoveFromDatabaseAsync(InventoryItem existingItem, CancellationToken cancellationToken)
    {
        _dbContext.InventoryItems.Remove(existingItem);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
