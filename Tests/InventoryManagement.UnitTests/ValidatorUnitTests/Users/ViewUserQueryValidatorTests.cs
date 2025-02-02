using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Users.ViewUser;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Users;

[TestFixture]
public class ViewUserQueryValidatorTests
{
    private ViewUserQueryValidator _vaidator;

    [SetUp]
    public void Setup()
    {
        _vaidator = new ViewUserQueryValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidUserIdSource))]
    public async Task Validate_WithInvalidUserId_ShouldReturnValidationFailure(int userId)
    {
        // Arrange
        UserIdQuery query = new(userId);

        // Act
        ValidationResult validationResult = await _vaidator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.ValidUserIdSource))]
    public async Task Validate_WithValidUserId_ShouldReturnValidationSuccess(int userId)
    {
        // Arrange
        UserIdQuery query = new(userId);

        // Act
        ValidationResult validationResult = await _vaidator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }
}
