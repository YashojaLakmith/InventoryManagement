
using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.InventoryItems.ListItems;

public class ListItemQueryHandler : IRequestHandler<ListItemsQuery, Result<ListItemsResult>>
{
    private readonly IValidator<ListItemsQuery> _validator;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ListItemQueryValidator> _logger;

    public ListItemQueryHandler(IValidator<ListItemsQuery> validator, ApplicationDbContext dbContext, ILogger<ListItemQueryValidator> logger)
    {
        _validator = validator;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<ListItemsResult>> Handle(ListItemsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError<ListItemsResult>(validationResult.Errors);
        }

        int offset = (request.PageNumber - 1) * request.RecordsPerPage;
        int take = request.RecordsPerPage;

        IQueryable<InventoryItem> query = _dbContext.InventoryItems.AsNoTracking();
        query = ApplyStringSearchFilter(query, request.ItemNamePartToSearch);

        List<ListItem> items = await query
            .Select(item => new ListItem(item.InventoryItemId, item.ItemName))
            .OrderBy(item => item.ItemId)
            .Skip(offset)
            .Take(take)
            .ToListAsync(cancellationToken);

        int count = await query.CountAsync(cancellationToken);

        return new ListItemsResult(items, items.Count, request.PageNumber, count);
    }

    private static IQueryable<InventoryItem> ApplyStringSearchFilter(IQueryable<InventoryItem> query, string? namePartToSearch)
    {
        return namePartToSearch == string.Empty || string.IsNullOrWhiteSpace(namePartToSearch)
            ? query
            : query.Where(item => EF.Functions.Like(item.ItemName, $"%{namePartToSearch}%"));
    }
}
