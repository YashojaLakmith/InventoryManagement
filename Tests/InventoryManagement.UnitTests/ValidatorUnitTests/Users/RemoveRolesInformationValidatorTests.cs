using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Users.RemoveRoles;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Users;

[TestFixture]
public class RemoveRolesInformationValidatorTests
{
    private RemoveRoleInformationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new RemoveRoleInformationValidator();
    }

    [Test]
    public async Task Validate_WithEmptyRoleArray_ShouldReturnValidationFailure()
    {
        // Arrange
        RemoveRoleInformation information = new(
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
        RemoveRoleInformation information = new(
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
        RemoveRoleInformation information = new(
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
        RemoveRoleInformation information = new(
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
        RemoveRoleInformation information = new(
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
        RemoveRoleInformation information = new(
            ValidatorTestCaseSources.ValidUserIdSource().First(),
            [roleName!]);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidRemoveRoleInformationSource))]
    public async Task Validate_WithValidInformation_ShouldReturnValidationSuccess(RemoveRoleInformation roleInformation)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(roleInformation);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    private static IEnumerable<RemoveRoleInformation> ValidRemoveRoleInformationSource()
    {
        using IEnumerator<int> userIdEnumerator = ValidatorTestCaseSources.ValidUserIdSource().GetEnumerator();
        using IEnumerator<string> roleNameEnumerator = ValidatorTestCaseSources.ValidRoleNameSource().GetEnumerator();

        while (userIdEnumerator.MoveNext() && roleNameEnumerator.MoveNext())
        {
            yield return new RemoveRoleInformation(userIdEnumerator.Current, [roleNameEnumerator.Current]);
        }
    }
}
