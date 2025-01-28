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
        yield return -55;
        yield return int.MaxValue;
        yield return int.MinValue;
        yield return 0;
        yield return -1;
    }

    private static IEnumerable<int> ValidPageNumberSource()
    {
        yield return 1;
        yield return int.MaxValue - 1;
        yield return 10;
        yield return 1007;
        yield return 99;
    }

    private static IEnumerable<int> InvalidResultsPerPageSourceByLowerBound()
    {
        yield return 9;
        yield return int.MinValue;
        yield return 0;
        yield return -10;
        yield return 5;
    }

    private static IEnumerable<int> InvalidResultsPerPageSourceByUpperBound()
    {
        yield return 101;
        yield return int.MaxValue;
        yield return 1000;
        yield return 404;
        yield return 804692;
    }

    private static IEnumerable<int> ValidResultsPerPageSource()
    {
        yield return 10;
        yield return 100;
        yield return 53;
        yield return 91;
        yield return 23;
    }
}
