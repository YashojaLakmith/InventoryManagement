using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.InventoryItems.CreateItem;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.InventoryItems;

[TestFixture]
public class NewItemInformationValidatorTests
{
    private NewItemInformationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new NewItemInformationValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidItemIdSource))]
    public async Task Validate_WithInvalidItemId_ShouldReturnValidationFailure(string? itemId)
    {
        // Arrange
        NewItemInformation information = new(
            itemId!,
            ValidatorTestCaseSources.ValidItemNameSource().First(),
            ValidatorTestCaseSources.ValidMeasurementUnitSource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidItemNameSource))]
    public async Task Validate_WithInvalidItemName_ShouldReturnValidationFailure(string? itemName)
    {
        // Arrange
        NewItemInformation information = new(
            ValidatorTestCaseSources.ValidItemNumberSource().First(),
            itemName!,
            ValidatorTestCaseSources.ValidMeasurementUnitSource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidMeasurementUnitSource))]
    public async Task Validate_WithInvalidMeasurementUnit_ShouldReturnValidationFailure(string? measurementUnit)
    {
        // Arrange
        NewItemInformation information = new(
            ValidatorTestCaseSources.ValidItemNumberSource().First(),
            ValidatorTestCaseSources.ValidItemNameSource().First(),
            measurementUnit!);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidNewItemInformationSource))]
    public async Task Validate_WithValidNewItemInformation_ShouldReturnValidationSuccess(NewItemInformation information)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    private static IEnumerable<NewItemInformation> ValidNewItemInformationSource()
    {
        using IEnumerator<string> itemIdSourceEnumerator = ValidatorTestCaseSources.ValidItemNumberSource().GetEnumerator();
        using IEnumerator<string> itemNameSourceEnumerator = ValidatorTestCaseSources.ValidItemNameSource().GetEnumerator();
        using IEnumerator<string> measurementUnitSourceEnumerator = ValidatorTestCaseSources.ValidMeasurementUnitSource().GetEnumerator();

        while (itemIdSourceEnumerator.MoveNext()
            && itemNameSourceEnumerator.MoveNext()
            && measurementUnitSourceEnumerator.MoveNext())
        {
            yield return new NewItemInformation(
                itemIdSourceEnumerator.Current,
                itemNameSourceEnumerator.Current,
                measurementUnitSourceEnumerator.Current);
        }
    }
}
