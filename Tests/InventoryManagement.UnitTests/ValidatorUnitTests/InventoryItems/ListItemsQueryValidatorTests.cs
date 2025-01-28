using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.InventoryItems.ListItems;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.InventoryItems;

[TestFixture]
public class ListItemsQueryValidatorTests
{
    private ListItemQueryValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new ListItemQueryValidator();
    }

    [Test, TestCaseSource(nameof(InvalidPageNumberSource))]
    public async Task Validate_WithInvalidPageNumber_ShouldReturnValidationFailure(int pageNumber)
    {
        // Arrange
        ListItemsQuery query = new(
            pageNumber,
            ValidRecordsPerPageSource().First(),
            ValidItemNamePartSource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(InvalidRecordsPerPageSource))]
    public async Task Validate_WithInvalidRecordsPerPage_ShouldReturnValidationFailure(int recordsPerPage)
    {
        // Arrange
        ListItemsQuery query = new(
            ValidPageNumberSource().First(),
            recordsPerPage,
            ValidItemNamePartSource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(InvalidItemNamePartSource))]
    public async Task Validate_WithInvalidItemNamePart_ShouldReturnValidationFailure(string? namePartToSearch)
    {
        // Arrange
        ListItemsQuery query = new(
            ValidPageNumberSource().First(),
            ValidRecordsPerPageSource().First(),
            namePartToSearch);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidListItemsQuerySource))]
    public async Task Validate_WithValidQueryData_ShouldReturnValidationSuccess(ListItemsQuery query)
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
        yield return int.MaxValue;
        yield return -1;
        yield return -74;
        yield return int.MinValue;
    }

    private static IEnumerable<int> InvalidRecordsPerPageSource()
    {
        yield return 9;
        yield return 0;
        yield return -43;
        yield return int.MinValue;
        yield return 101;
        yield return 300;
        yield return int.MaxValue;
    }

    private static IEnumerable<int> ValidPageNumberSource()
    {
        yield return 1;
        yield return int.MaxValue - 1;
        yield return 101;
        yield return 40135;
        yield return 97461;
    }

    private static IEnumerable<int> ValidRecordsPerPageSource()
    {
        yield return 10;
        yield return 100;
        yield return 94;
        yield return 35;
        yield return 67;
    }

    private static IEnumerable<string?> ValidItemNamePartSource()
    {
        yield return null;
        yield return string.Empty;
        yield return "     ";

        foreach (string name in ValidatorTestCaseSources.ValidItemNameSource())
        {
            yield return name;
        }
    }

    private static IEnumerable<string> InvalidItemNamePartSource()
    {
        yield return "a1bcvc   jhcsjhkyusqhdbn d cddggdd  4763re  fw dhqadhqd wnmfwtra5xs cm dvsdfqytd";
        yield return "X SVUFYG87B VC  uyydt7ef7eddv jhTWVF ENMF YT7vwfw dhgfyt   HGDAVD Q DGFSTYAS  ftsdx ";
        yield return "nbfhdjfjsbfdsbfdfnstglqit64bx'7-cga21,b trnc=g5ras8vnduf903jrj3rr";
        yield return "1dfhugsydsbdsjdvFYGDJKWSMJLWHUgbdsjnfkshuHDS'DOOWSDUJD W83749Jknate45243hhej";
        yield return "524783402-44770-2039473834-27428nmbdfgujh543feuiyf67rysdi67634hrgedmv kdjvbi";
    }

    private static IEnumerable<ListItemsQuery> ValidListItemsQuerySource()
    {
        using IEnumerator<int> pageNumberSourceEnumerator = ValidPageNumberSource().GetEnumerator();
        using IEnumerator<int> recordsPerPageSourceEnumerator = ValidRecordsPerPageSource().GetEnumerator();
        using IEnumerator<string?> namePartSourceEnumerator = ValidItemNamePartSource().GetEnumerator();

        while (
            pageNumberSourceEnumerator.MoveNext()
            && recordsPerPageSourceEnumerator.MoveNext()
            && namePartSourceEnumerator.MoveNext())
        {
            yield return new ListItemsQuery(
                pageNumberSourceEnumerator.Current,
                recordsPerPageSourceEnumerator.Current,
                namePartSourceEnumerator.Current);
        }
    }
}
