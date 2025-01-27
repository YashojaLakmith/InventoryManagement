using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

namespace InventoryManagement.Api.Features.InventoryItems.ListItems;

public class ListItemQueryHandler : IRequestHandler<ListItemsQuery, Result<ListItemsResult>>
{
    private readonly IValidator<ListItemsQuery> _validator;
    private readonly IInventoryItemRepository _repository;
    private readonly ILogger<ListItemQueryValidator> _logger;

    public ListItemQueryHandler(IValidator<ListItemsQuery> validator, IInventoryItemRepository repository, ILogger<ListItemQueryValidator> logger)
    {
        _validator = validator;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ListItemsResult>> Handle(ListItemsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        return !validationResult.IsValid
            ? InvalidDataError.CreateFailureResultFromError<ListItemsResult>(validationResult.Errors)
            : await _repository.ListItemsAsync(request.PageNumber, request.RecordsPerPage, request.ItemNamePartToSearch, cancellationToken);
    }
}
