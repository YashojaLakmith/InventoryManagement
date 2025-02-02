using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Shared.Abstractions;
using InventoryManagement.Api.Features.Transactions;
using InventoryManagement.Api.Features.Transactions.CreateTransactionReport;

using Moq;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Transactions;

[TestFixture]
public class TransactionReportFilterValidatorTests
{
    private Mock<ITimeProvider> _timeProviderMock;
    private TransactionReportFilterValidator _validator;
    private static readonly DateTime FixedCurrentDate = new(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime FixedValidSinceDate = FixedCurrentDate - TimeSpan.FromDays(30);

    [SetUp]
    public void Setup()
    {
        _timeProviderMock = new Mock<ITimeProvider>();
        _validator = new TransactionReportFilterValidator(_timeProviderMock.Object);
    }

    [Test, TestCaseSource(nameof(InvalidDateBecauseFutureDateSource))]
    public async Task Validate_WithSinceDateAsAFutureDate_ShouldReturnValidationFailure(DateTime sinceDate)
    {
        // Arrange
        _timeProviderMock.Setup(t => t.CurrentUtcTime)
            .Returns(FixedCurrentDate);

        TransactionReportFilters filter = new(sinceDate, FixedCurrentDate, ValidTransactionTypeSource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(InvalidToDateBecauseEarlierThanSinceDateSource))]
    public async Task Validate_WithToDateEarlierThanSinceDate_ShouldReturnValidationFailure(DateTime toDate)
    {
        // Arrange
        _timeProviderMock.Setup(t => t.CurrentUtcTime)
            .Returns(FixedCurrentDate);

        TransactionReportFilters filter = new(FixedValidSinceDate, toDate, ValidTransactionTypeSource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(InvalidDateBecauseFutureDateSource))]
    public async Task Validate_WithToDateAsAFutureDate_ShouldReturnValidationFailure(DateTime toDate)
    {
        // Arrange
        _timeProviderMock.Setup(t => t.CurrentUtcTime)
            .Returns(FixedCurrentDate);

        TransactionReportFilters filter = new(FixedValidSinceDate, toDate, ValidTransactionTypeSource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(NullOrEmptyTransactionTypeArray))]
    public async Task Validate_WithTransactionTypesArrayNullOrEmpty_ShouldReturnValidationFailure(string[]? transactionTypes)
    {
        // Arrange
        _timeProviderMock.Setup(t => t.CurrentUtcTime)
            .Returns(FixedCurrentDate);

        TransactionReportFilters filter = new(FixedValidSinceDate, FixedCurrentDate, transactionTypes!);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(InvalidTransactionTypesInArraySource))]
    public async Task Validate_WithInvalidTransactionTypesInArray_ShouldReturnValidationFailure(string[] transactionTypes)
    {
        // Arrange
        _timeProviderMock.Setup(t => t.CurrentUtcTime)
            .Returns(FixedCurrentDate);

        TransactionReportFilters filter = new(FixedValidSinceDate, FixedCurrentDate, transactionTypes);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidFilterSource))]
    public async Task Validate_ValidTransactionFilters_ShouldReturnValidationSuccess(TransactionReportFilters filter)
    {
        // Arrange
        _timeProviderMock.Setup(t => t.CurrentUtcTime)
            .Returns(FixedCurrentDate);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(filter);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    private static IEnumerable<DateTime> InvalidDateBecauseFutureDateSource()
    {
        yield return FixedCurrentDate + TimeSpan.FromDays(1);
        yield return FixedCurrentDate + TimeSpan.FromDays(100);
        yield return FixedCurrentDate + TimeSpan.FromDays(365);
        yield return FixedCurrentDate + TimeSpan.FromDays(863);
        yield return FixedCurrentDate + TimeSpan.FromDays(3650);
    }

    private static IEnumerable<DateTime> InvalidToDateBecauseEarlierThanSinceDateSource()
    {
        yield return FixedValidSinceDate - TimeSpan.FromDays(1);
        yield return FixedValidSinceDate - TimeSpan.FromDays(100);
        yield return FixedValidSinceDate - TimeSpan.FromDays(365);
        yield return FixedValidSinceDate - TimeSpan.FromDays(863);
        yield return FixedValidSinceDate - TimeSpan.FromDays(3650);
    }

    private static IEnumerable<string[]> ValidTransactionTypeSource()
    {
        yield return [InventoryTransactionTypes.Receive];
        yield return [InventoryTransactionTypes.Issue];
        yield return [InventoryTransactionTypes.Receive, InventoryTransactionTypes.Issue];
        yield return [InventoryTransactionTypes.Issue, InventoryTransactionTypes.Receive];
    }

    private static IEnumerable<string[]?> NullOrEmptyTransactionTypeArray()
    {
        yield return null;
        yield return [];
    }

    private static IEnumerable<string[]> InvalidTransactionTypesInArraySource()
    {
        yield return ["Blah"];
        yield return [InventoryTransactionTypes.Receive, InventoryTransactionTypes.Receive];
        yield return [InventoryTransactionTypes.Receive, InventoryTransactionTypes.Issue, InventoryTransactionTypes.Issue];
        yield return ["gdyegeheu", "dbeg35tr3"];
        yield return ["", ""];
        yield return [null!, null!];
        yield return ["     ", "     "];
    }

    private static IEnumerable<TransactionReportFilters> ValidFilterSource()
    {
        foreach (string[] types in ValidTransactionTypeSource())
        {
            yield return new TransactionReportFilters(FixedValidSinceDate, FixedCurrentDate, types);
        }
    }
}
