using System.Security.Cryptography;

using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Batches.CreateNewBatch;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Batches;

[TestFixture]
public class NewBatchInformationValidatorTests
{
    private NewBatchInformationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new NewBatchInformationValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidBatchNumberSource))]
    public async Task Validate_WithInvalidBatchNumber_ShouldReturnValidationFailure(string? invalidBatchNumber)
    {
        // Arrange
        const int sampleSize = 5;
        List<ItemOrder> sampleOrders = CreateSampleOfValidItemOrders(sampleSize);
        NewBatchInformation information = new(invalidBatchNumber!, sampleOrders);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test]
    public async Task Validate_WithItemOrdersAreEmpty_ShouldReturnValidationFailure()
    {
        // Arrange
        NewBatchInformation information = new(@"VALID_BATCH_01", []);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test]
    public async Task Validate_WithDuplicateItemsExistInItemOrders_ShouldReturnValidationFailure()
    {
        // Arrange
        const int sampleSize = 5;
        List<ItemOrder> orders = CreateSampleOfValidItemOrders(sampleSize);
        orders.Add(orders[2]);
        NewBatchInformation information = new(@"VALID_BATCH_01", orders);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidItemIdSource))]
    public async Task Validate_WithInvalidInventoryItemIdInItemOrders_ShouldReturnValidationFailure(string? invalidItemId)
    {
        // Arrange
        ItemOrder order = new(invalidItemId!, 5, 50);
        NewBatchInformation information = new(@"VALID_BATCH_01", [order]);

        // Act
        ValidationResult result = await _validator.ValidateAsync(information);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidPerUnitPriceSource))]
    public async Task Validate_WithInvalidPerUnitPriceInItemOrders_ShouldReturnValidationFailure(decimal perUnitPrice)
    {
        // Arrange
        ItemOrder order = new(@"VALID_ITEM_01", 5, perUnitPrice);
        NewBatchInformation information = new(@"VALID_BATCH_01", [order]);

        // Act
        ValidationResult result = await _validator.ValidateAsync(information);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidQuantitySource))]
    public async Task Validate_WithInvalidQuantityPerItemOrder_ShouldReturnValidationFailure(int quantity)
    {
        // Arrange
        ItemOrder order = new(@"VALID_ITEM_01", quantity, 10);
        NewBatchInformation information = new(@"VALID_BATCH_01", [order]);

        // Act
        ValidationResult result = await _validator.ValidateAsync(information);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.ValidBatchNumberSource))]
    public async Task Validate_WithValidBatchNumberAndItemOrders_ShouldReturnValidationSuccess(string batchNumber)
    {
        // Arrange
        List<ItemOrder> orders = CreateValidItemOrders();
        NewBatchInformation information = new(batchNumber, orders);

        // Act
        ValidationResult result = await _validator.ValidateAsync(information);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    private static List<ItemOrder> CreateSampleOfValidItemOrders(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        List<ItemOrder> samples = new(count);

        for (int i = 1; i <= count; i++)
        {
            ItemOrder order = new($"NEW_ITEM{i}", i * 10, i * 100);
            samples.Add(order);
        }

        return samples;
    }

    private static List<ItemOrder> CreateValidItemOrders()
    {
        List<ItemOrder> orders = [];

        foreach (string itemNumber in ValidatorTestCaseSources.ValidItemNumberSource())
        {
            decimal price = RandomNumberGenerator.GetInt32(1, int.MaxValue);
            int quantity = RandomNumberGenerator.GetInt32(1, int.MaxValue);
            ItemOrder order = new(itemNumber, quantity, price);

            orders.Add(order);
        }

        return orders.Count == 0
            ? throw new InvalidOperationException(@"Valid item orders are 0")
            : orders;
    }
}
