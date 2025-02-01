using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Users.ListUsers;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Users;

[TestFixture]
public class ListUserQueryValidatorTests
{
    private ListUserQueryValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new ListUserQueryValidator();
    }

    [Test, TestCaseSource(nameof(InvalidPageNumberSource))]
    public async Task Validate_WhenInvalidPageNumber_ShouldReturnValidationFailure(int pageNumber)
    {
        // Arrange
        ListUserQuery query = new(
            ValidResultsPerPageSource().First(),
            pageNumber,
            [.. ValidatorTestCaseSources.ValidRoleNameSource()]);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(InvalidResultsPerPageSource))]
    public async Task Validate_WhenInvalidResultsPerPage_ShouldReturnValidationFailure(int resultsPerPage)
    {
        // Arrange
        ListUserQuery query = new(
            resultsPerPage,
            ValidPageNumberSource().First(),
            [.. ValidatorTestCaseSources.ValidRoleNameSource()]);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidRoleNameSource))]
    public async Task Validate_WhenInvalidRoleNameInArray_ShouldReturnValidationFailure(string? roleName)
    {
        // Arrange
        ListUserQuery query = new(
            ValidResultsPerPageSource().First(),
            ValidPageNumberSource().First(),
            [roleName!]);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test]
    public async Task Validate_WhenDuplicatedRoleNamesInArray_ShouldReturnValidationFailure()
    {
        // Arrange
        ListUserQuery query = new(
            ValidResultsPerPageSource().First(),
            ValidPageNumberSource().First(),
            [.. ValidatorTestCaseSources.DuplicateRoleNameSource()]);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidQuerySource))]
    public async Task Validate_WhenValidQuery_ShouldReturnValidationSuccess(ListUserQuery query)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    private static IEnumerable<int> InvalidPageNumberSource()
    {
        yield return 0;
        yield return int.MinValue;
        yield return -1;
        yield return int.MaxValue;
        yield return -376;
    }

    private static IEnumerable<int> ValidResultsPerPageSource()
    {
        yield return 10;
        yield return 25;
        yield return 15;
        yield return 20;
    }

    private static IEnumerable<int> ValidPageNumberSource()
    {
        yield return 1;
        yield return int.MaxValue - 1;
        yield return 100;
        yield return 5246;
        yield return 87234;
    }

    private static IEnumerable<int> InvalidResultsPerPageSource()
    {
        yield return 9;
        yield return 0;
        yield return -6;
        yield return 26;
        yield return 100;
        yield return int.MaxValue;
    }

    private static IEnumerable<ListUserQuery> ValidQuerySource()
    {
        using IEnumerator<int> resultsPerPageEnumerator = ValidResultsPerPageSource().GetEnumerator();
        using IEnumerator<int> pageNumberEnumerator = ValidPageNumberSource().GetEnumerator();
        using IEnumerator<string> roleNameEnumerator = ValidatorTestCaseSources.ValidRoleNameSource().GetEnumerator();

        while (resultsPerPageEnumerator.MoveNext() && pageNumberEnumerator.MoveNext() && roleNameEnumerator.MoveNext())
        {
            yield return new ListUserQuery(
                resultsPerPageEnumerator.Current,
                pageNumberEnumerator.Current,
                [roleNameEnumerator.Current]);
        }
    }
}
