using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Authentication.ResetPassword;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Authentication;

[TestFixture]
public class PasswordResetTokenValidatorTests
{
    private PasswordResetTokenValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new PasswordResetTokenValidator(SharedValidatorInstances.EmailValidator, SharedValidatorInstances.PasswordValidator);
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidEmailSource))]
    public async Task Validate_WithInvalidEmail_ShouldReturnValidationFailure(string? invalidEmail)
    {
        // Arrange
        PasswordResetTokenData data = new(invalidEmail!, @"ValidToken", @"ValidPassword1");

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(data);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [TestCase(@"")]
    [TestCase(@"    ")]
    [TestCase(null)]
    public async Task Vaidate_WithInvalidToken_ShouldReturnValidationFailure(string? invalidToken)
    {
        // Arrange
        PasswordResetTokenData data = new(@"testmail@test.mail", invalidToken!, @"ValidPassword1");

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(data);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidPasswordSource))]
    public async Task Validate_WithInvalidNewPassword_ShouldReturnValidationFailure(string? invalidPassword)
    {
        // Arrange
        PasswordResetTokenData data = new(@"testmail@test.mail", @"ValidToken123", invalidPassword!);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(data);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.ValidPasswordResetDataSource))]
    public async Task Validate_WithValidEmailTokenAndNewPassword_ShouldReturnValidationSuccess(PasswordResetTokenData data)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(data);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }
}
