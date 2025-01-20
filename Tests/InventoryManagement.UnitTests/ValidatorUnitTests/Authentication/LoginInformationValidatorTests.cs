using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Authentication.Login;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Authentication;

[TestFixture]
public class LoginInformationValidatorTests
{
    private LoginInformationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new LoginInformationValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidEmailSource))]
    public async Task Validate_WithInvalidEmail_ShouldReturnValidationFailure(string? emailAdress)
    {
        // Arrange
        LoginInformation sampleLoginInformation = new(emailAdress!, @"ValidPassword1");

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(sampleLoginInformation);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidPasswordSource))]
    public async Task Validate_WithInvalidPassword_ShouldReturnValidationFailure(string? password)
    {
        // Arrange
        LoginInformation sampleLoginInformation = new(@"testmail@email.test", password!);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(sampleLoginInformation);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.ValidEmailAndPasswordSource))]
    public async Task Validate_WithValidEmailAndPassword_ShouldReturnValidationSuccess(LoginInformation loginInformation)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(loginInformation);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }
}
