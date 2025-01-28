using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.InventoryItems.ViewItem;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.InventoryItems;

[TestFixture]
public class ViewItemQueryValidatorTests
{
    private ViewItemQueryValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new ViewItemQueryValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidItemIdSource))]
    public async Task Validate_WithInvalidItemId_ShouldReturnValidationFailure(string? itemId)
    {
        // Arrange
        ViewItemQuery query = new(itemId!);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.ValidItemNumberSource))]
    public async Task Validate_WithValidItemId_ShouldReturnValidationSuccess(string itemId)
    {
        // Arrange
        ViewItemQuery query = new(itemId);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }
}
