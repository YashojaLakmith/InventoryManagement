using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Users.RemoveUser;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Users;

[TestFixture]
public class RemoveUserInformationValidatorTests
{
    private RemoveUserInformationValidator _vaidator;

    [SetUp]
    public void Setup()
    {
        _vaidator = new();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidUserIdSource))]
    public async Task Validate_WithInvalidUserId_ShouldReturnValidationFailure(int userId)
    {
        // Arrange
        RemoveUserInformation query = new(userId);

        // Act
        ValidationResult validationResult = await _vaidator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.ValidUserIdSource))]
    public async Task Validate_WithValidUserId_ShouldReturnValidationSuccess(int userId)
    {
        // Arrange
        RemoveUserInformation query = new(userId);

        // Act
        ValidationResult validationResult = await _vaidator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }
}
