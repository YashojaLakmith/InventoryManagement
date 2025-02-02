using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.InventoryItems.DeleteItem;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.InventoryItems;

[TestFixture]
public class DeleteItemCommandValidatorTests
{
    private ItemIdToDeleteValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new ItemIdToDeleteValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidItemIdSource))]
    public async Task Validate_WithInvalidItemId_ShouldReturnValidationFailure(string? itemId)
    {
        // Arrange
        ItemIdToDelete command = new(itemId!);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.ValidItemNumberSource))]
    public async Task Validate_WithValidItemId_ShouldReturnValidationSuccess(string itemId)
    {
        // Arrange
        ItemIdToDelete command = new(itemId);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(command);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }
}
