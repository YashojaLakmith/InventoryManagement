using System.Security.Cryptography;

using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Batches.ListBatchesByBatchNumber;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Batches;

[TestFixture]
public class BatchNumberFilterValidatorTests
{
    private BatchNumberFilterValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new BatchNumberFilterValidator();
    }

    [Test]
    public async Task Validate_WithItemNumberAsNull_ShouldReturnValidationSuccess()
    {
        // Arrange
        BatchNumberFilter filter = new(null, true, 1, 1);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidItemIdSourceWithoutNull))]
    public async Task Validate_WithItemNumberIsNotNullAndInvalidItemNumber_ShouldReturnValidationFailure(string itemNumber)
    {
        // Arrange
        BatchNumberFilter filter = new(itemNumber, true, 1, 1);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.ValidItemNumberSource))]
    public async Task Validate_WithValidItemNumber_ShouldReturnValidationSuccess(string itemNumber)
    {
        // Arrange
        BatchNumberFilter filter = new(itemNumber, true, 1, 1);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Validate_WithAnyBooleanValueAsIgnoreInactiveFlag_ShouldReturnValidationSuccess(bool ignoreInactive)
    {
        // Arrange
        BatchNumberFilter filter = new(@"VALID_ITEM_01", ignoreInactive, 1, 1);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    [Test, TestCaseSource(nameof(InvalidPageNumberSource))]
    public async Task Validate_WithPageNumberLessThanOne_ShouldReturnValidationFailure(int pageNumber)
    {
        // Arrange
        BatchNumberFilter filter = new(@"VALID_ITEM_01", true, pageNumber, 1);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test]
    public async Task Validate_WithPageNumberEqualsIntegerUpperLimit_ShouldReturnValidationFailure()
    {
        // Arrange
        BatchNumberFilter filter = new(@"VALID_ITEM_01", true, int.MaxValue, 1);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(InvalidResultsPerPageSourceByLowerBound))]
    public async Task Validate_WithResultsPerPageLessThanTen_ShouldReturnValidationFailure(int resultsPerPage)
    {
        // Arrange
        BatchNumberFilter filter = new(@"VALID_ITEM_01", true, 1, resultsPerPage);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(InvalidResultsPerPageSourceByUpperBound))]
    public async Task Validate_WithResultsPerPageMoreThanHundred_ShouldReturnValidationFailure(int resultsPerPage)
    {
        // Arrange
        BatchNumberFilter filter = new(@"VALID_ITEM_01", true, 1, resultsPerPage);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidResultsPerPageSource))]
    public async Task Validate_WithValidResultsPerPage_ShouldReturnValidationSuccess(int resultsPerPage)
    {
        // Arrange
        BatchNumberFilter filter = new(@"VALID_ITEM_01", true, 1, resultsPerPage);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    [Test, TestCaseSource(nameof(ValidPageNumberSource))]
    public async Task Validate_WithValidPageNumber_ShouldReturnValidationSuccess(int pageNumber)
    {
        // Arrange
        BatchNumberFilter filter = new(@"VALID_ITEM_01", true, pageNumber, 1);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    private static IEnumerable<int> InvalidPageNumberSource()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return RandomNumberGenerator.GetInt32(int.MinValue, 1);
        }
    }

    private static IEnumerable<int> ValidPageNumberSource()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return RandomNumberGenerator.GetInt32(1, int.MaxValue);
        }
    }

    private static IEnumerable<int> InvalidResultsPerPageSourceByLowerBound()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return RandomNumberGenerator.GetInt32(int.MinValue, 10);
        }
    }

    private static IEnumerable<int> InvalidResultsPerPageSourceByUpperBound()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return RandomNumberGenerator.GetInt32(101, int.MaxValue);
        }
    }

    private static IEnumerable<int> ValidResultsPerPageSource()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return RandomNumberGenerator.GetInt32(10, 101);
        }
    }
}
