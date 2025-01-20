using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Batches.DeleteBatchLine;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Batches;

[TestFixture]
public class DeleteBatchLineCommandValidatorTests
{
    private DeleteBatchLineCommandValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new DeleteBatchLineCommandValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidBatchNumberSource))]
    public async Task Validate_WithInvalidBatchNumber_ShouldReturnValidationFailure(string? invalidBatchNumber)
    {
        // Arrange
        DeleteBatchLineCommand command = new(invalidBatchNumber!, @"VALID_ITEM_NUMBER");

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidItemIdSource))]
    public async Task Validate_WithInvalidItemNumber_ShouldReturnValidationFailure(string? invalidItemNumber)
    {
        // Arrange
        DeleteBatchLineCommand command = new(@"VALID_BATCH_ID", invalidItemNumber!);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidBatchLineCommandSource))]
    public async Task Validate_WithValidDeleteBatchLineCommand_ShouldReturnValidationSuccess(DeleteBatchLineCommand command)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(command);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    private static IEnumerable<DeleteBatchLineCommand> ValidBatchLineCommandSource()
    {
        using IEnumerator<string> batchNumberEnumerator = ValidatorTestCaseSources.ValidBatchNumberSource().GetEnumerator();
        using IEnumerator<string> itemNumberEnumerator = ValidatorTestCaseSources.ValidItemNumberSource().GetEnumerator();

        while (batchNumberEnumerator.MoveNext() && itemNumberEnumerator.MoveNext())
        {
            yield return new DeleteBatchLineCommand(batchNumberEnumerator.Current, itemNumberEnumerator.Current);
        }
    }
}
