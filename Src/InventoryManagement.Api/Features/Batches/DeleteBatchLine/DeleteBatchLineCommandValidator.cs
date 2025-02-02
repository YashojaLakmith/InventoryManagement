using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Batches.DeleteBatchLine;

public class DeleteBatchLineCommandValidator : AbstractValidator<DeleteBatchLineCommand>
{
    public DeleteBatchLineCommandValidator()
    {
        RuleFor(x => new BatchNumber(x.BatchNumber))
            .SetValidator(BatchNumberValidator.Instance);

        RuleFor(x => new InventoryItemNumber(x.ItemNumber))
            .SetValidator(InventoryItemNumberValidator.Instance);
    }
}
