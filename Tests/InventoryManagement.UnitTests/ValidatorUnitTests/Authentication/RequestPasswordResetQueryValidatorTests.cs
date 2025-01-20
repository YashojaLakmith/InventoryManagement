using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Authentication.RequestPasswordReset;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Authentication;

[TestFixture]
public class RequestPasswordResetQueryValidatorTests
{
    private RequestPasswordResetQueryValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new RequestPasswordResetQueryValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidEmailSource))]
    public async Task Validate_WithInvalidEmail_ShouldReturnValidationFailure(string? invalidEmail)
    {
        // Arrange
        RequestPasswordResetQuery query = new(invalidEmail!);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.ValidEmailSource))]
    public async Task Validate_WithValidEmail_ShouldReturnValidationSuccess(string validEmail)
    {
        // Arrange
        RequestPasswordResetQuery query = new(validEmail);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(query);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }
}
