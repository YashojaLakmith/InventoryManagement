using FluentValidation;

using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.Api.Features.Batches.DeleteBatchLine;

public class DeleteBatchLineCommandValidator : AbstractValidator<DeleteBatchLineCommand>
{
    public DeleteBatchLineCommandValidator(IValidator<BatchNumber> batchNumberValidator)
    {
        RuleFor(x => new BatchNumber(x.BatchNumber))
            .SetValidator(batchNumberValidator);
    }
}
