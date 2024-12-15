using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Features.InventoryItems.Errors;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

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

        ValidationResult validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new NotImplementedException();
        }

        InventoryItem? existingItem = await _dbContext.InventoryItems.FindAsync(request.ItemId);
        if (existingItem is null)
        {
            return Result.Fail(new ItemNotFoundError());
        }

        _dbContext.InventoryItems.Remove(existingItem);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
