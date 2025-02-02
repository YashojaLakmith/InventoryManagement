using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Transactions.GoodsIssuance;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Transactions;

[TestFixture]
public class IssuanceInformationValidatorTests
{
    private IssuanceInformationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new IssuanceInformationValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidBatchNumberSource))]
    public async Task Validate_WithInvalidBatchNumber_ShouldReturnValidationFailure(string? batchNumber)
    {
        // Arrange
        IssuanceInformation information = new(
            ValidatorTestCaseSources.ValidItemNumberSource().First(),
            batchNumber!,
            ValidQuantitySource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidItemIdSource))]
    public async Task Validate_WithInvalidItemId_ShouldReturnValidationFailure(string? itemId)
    {
        // Arrange
        IssuanceInformation information = new(
            itemId!,
            ValidatorTestCaseSources.ValidBatchNumberSource().First(),
            ValidQuantitySource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(InvalidQuantitySource))]
    public async Task Validate_WithInvalidQuantity_ShouldReturnValidationFailure(int quantity)
    {
        // Arrange
        IssuanceInformation information = new(
            ValidatorTestCaseSources.ValidItemNumberSource().First(),
            ValidatorTestCaseSources.ValidBatchNumberSource().First(),
            quantity);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidIssuanceInformationSource))]
    public async Task Validate_WithValidIssuanceInformation_ShouldReturnValidationSuccess(IssuanceInformation information)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    private static IEnumerable<int> InvalidQuantitySource()
    {
        yield return 0;
        yield return int.MaxValue;
        yield return int.MinValue;
        yield return -1;
        yield return -27;
    }

    private static IEnumerable<IssuanceInformation> ValidIssuanceInformationSource()
    {
        using IEnumerator<int> quantityEnumerator = ValidQuantitySource().GetEnumerator();
        using IEnumerator<string> batchNumberEnumerator = ValidatorTestCaseSources.ValidBatchNumberSource().GetEnumerator();
        using IEnumerator<string> itemIdEnumerator = ValidatorTestCaseSources.ValidItemNumberSource().GetEnumerator();

        while (
            quantityEnumerator.MoveNext()
            && batchNumberEnumerator.MoveNext()
            && itemIdEnumerator.MoveNext())
        {
            yield return new IssuanceInformation(itemIdEnumerator.Current, batchNumberEnumerator.Current, quantityEnumerator.Current);
        }
    }

    private static IEnumerable<int> ValidQuantitySource()
    {
        yield return 1;
        yield return int.MaxValue - 1;
        yield return 19;
        yield return 257016;
        yield return 896;
    }
}
