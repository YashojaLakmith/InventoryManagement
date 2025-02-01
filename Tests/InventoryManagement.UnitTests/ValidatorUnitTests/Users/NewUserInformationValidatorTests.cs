using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Users.CreateUser;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Users;

[TestFixture]
public class NewUserInformationValidatorTests
{
    private NewUserInformationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new NewUserInformationValidator();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidUserNameSource))]
    public async Task Validate_WithInvalidUserName_ShouldReturnValidationFailure(string? userName)
    {
        // Arrange
        NewUserInformation information = new(
            userName!,
            ValidatorTestCaseSources.ValidEmailSource().First(),
            ValidatorTestCaseSources.ValidPasswordSource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidEmailSource))]
    public async Task Validate_WithInvalidEmail_ShouldReturnValidationFailure(string? email)
    {
        // Arrange
        NewUserInformation information = new(
            ValidatorTestCaseSources.ValidUserNameSource().First(),
            email!,
            ValidatorTestCaseSources.ValidPasswordSource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidPasswordSource))]
    public async Task Validate_WithInvalidPassword_ShouldReturnValidationFailure(string? password)
    {
        // Arrange
        NewUserInformation information = new(
            ValidatorTestCaseSources.ValidUserNameSource().First(),
            ValidatorTestCaseSources.ValidEmailSource().First(),
            password!);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidNewUserInformationSource))]
    public async Task Validate_WithValidUserInformation_ShouldReturnValidationSuccess(NewUserInformation information)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    private static IEnumerable<NewUserInformation> ValidNewUserInformationSource()
    {
        using IEnumerator<string> userNameEnumerator = ValidatorTestCaseSources.ValidUserNameSource().GetEnumerator();
        using IEnumerator<string> emailEnumerator = ValidatorTestCaseSources.ValidEmailSource().GetEnumerator();
        using IEnumerator<string> passwordEnumerator = ValidatorTestCaseSources.ValidPasswordSource().GetEnumerator();

        while (userNameEnumerator.MoveNext() && emailEnumerator.MoveNext() && passwordEnumerator.MoveNext())
        {
            yield return new NewUserInformation(userNameEnumerator.Current, emailEnumerator.Current, passwordEnumerator.Current);
        }
    }
}
