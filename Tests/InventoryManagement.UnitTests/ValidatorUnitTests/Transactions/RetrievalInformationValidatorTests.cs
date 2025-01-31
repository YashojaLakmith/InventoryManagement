using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Transactions.GoodsReceive;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Transactions;

[TestFixture]
public class RetrievalInformationValidatorTests
{
    private RetrievalInformationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new RetrievalInformationValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidBatchNumberSource))]
    public async Task Validate_WithInvalidBatchNumber_ShouldReturnValidationFailure(string? batchNumber)
    {
        // Arrange
        RetrievalInformation information = new(
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
        RetrievalInformation information = new(
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
        RetrievalInformation information = new(
            ValidatorTestCaseSources.ValidItemNumberSource().First(),
            ValidatorTestCaseSources.ValidBatchNumberSource().First(),
            quantity);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidIssuanceInformationSource))]
    public async Task Validate_WithValidIssuanceInformation_ShouldReturnValidationSuccess(RetrievalInformation information)
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

    private static IEnumerable<RetrievalInformation> ValidIssuanceInformationSource()
    {
        using IEnumerator<int> quantityEnumerator = ValidQuantitySource().GetEnumerator();
        using IEnumerator<string> batchNumberEnumerator = ValidatorTestCaseSources.ValidBatchNumberSource().GetEnumerator();
        using IEnumerator<string> itemIdEnumerator = ValidatorTestCaseSources.ValidItemNumberSource().GetEnumerator();

        while (
            quantityEnumerator.MoveNext()
            && batchNumberEnumerator.MoveNext()
            && itemIdEnumerator.MoveNext())
        {
            yield return new RetrievalInformation(itemIdEnumerator.Current, batchNumberEnumerator.Current, quantityEnumerator.Current);
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
