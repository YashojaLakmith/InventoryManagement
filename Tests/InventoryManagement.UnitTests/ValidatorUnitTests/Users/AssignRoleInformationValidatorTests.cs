using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Users.AssignRoles;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Users;

[TestFixture]
public class AssignRoleInformationValidatorTests
{
    private AssignRoleInformationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new AssignRoleInformationValidator();
    }

    [Test]
    public async Task Validate_WithEmptyRoleArray_ShouldReturnValidationFailure()
    {
        // Arrange
        AssignRoleInformation information = new(
            ValidatorTestCaseSources.ValidUserIdSource().First(),
            []);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidUserIdSource))]
    public async Task Validate_WithInvalidUserId_ShouldReturnValidationFailure(int userId)
    {
        // Arrange
        AssignRoleInformation information = new(
            userId,
            [.. ValidatorTestCaseSources.ValidRoleNameSource().Take(1)]);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.SuperUserRoleNameSource))]
    public async Task Validate_SuperUserContainingAsARoleToAssign_ShouldReturnValidationFailure(string roleNameForms)
    {
        // Arrange
        AssignRoleInformation information = new(
            ValidatorTestCaseSources.ValidUserIdSource().First(),
            [.. ValidatorTestCaseSources.ValidRoleNameSource().Take(1), roleNameForms]);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test]
    public async Task Validate_WhenDuplicateRoleNamesExistInArray_ShouldReturnValidationFailure()
    {
        // Arrange
        AssignRoleInformation information = new(
            ValidatorTestCaseSources.ValidUserIdSource().First(),
            [.. ValidatorTestCaseSources.DuplicateRoleNameSource()]);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test]
    public async Task Validate_WhenRoleCountExceedsFive_ShouldReturnValidationFailure()
    {
        // Arrange
        AssignRoleInformation information = new(
            ValidatorTestCaseSources.ValidUserIdSource().First(),
            [.. ValidatorTestCaseSources.ValidRoleNameSource()]);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidRoleNameSource))]
    public async Task Validate_WhenRoleNameIsInvalid_ShouldReturnValidationFailure(string? roleName)
    {
        // Arrange
        AssignRoleInformation information = new(
            ValidatorTestCaseSources.ValidUserIdSource().First(),
            [roleName!]);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidAssignRoleInformationSource))]
    public async Task Validate_WithValidInformation_ShouldReturnValidationSuccess(AssignRoleInformation roleInformation)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(roleInformation);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    private static IEnumerable<AssignRoleInformation> ValidAssignRoleInformationSource()
    {
        using IEnumerator<int> userIdEnumerator = ValidatorTestCaseSources.ValidUserIdSource().GetEnumerator();
        using IEnumerator<string> roleNameEnumerator = ValidatorTestCaseSources.ValidRoleNameSource().GetEnumerator();

        while (userIdEnumerator.MoveNext() && roleNameEnumerator.MoveNext())
        {
            yield return new AssignRoleInformation(userIdEnumerator.Current, [roleNameEnumerator.Current]);
        }
    }
}
