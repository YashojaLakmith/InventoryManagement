using FluentAssertions;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Users.ModifyPassword;

namespace InventoryManagement.UnitTests.ValidatorUnitTests.Users;

[TestFixture]
public class ModifyPasswordInformationValidatorTests
{
    private ModifyPasswordInformationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new ModifyPasswordInformationValidator();
    }

    [Test, TestCaseSource(nameof(EqualPasswordPairsSource))]
    public async Task Validate_WithBothPasswordsAreEqual_ShouldReturnValidationFailure(ModifyPasswordInformation information)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidPasswordSource))]
    public async Task Validate_WithInvalidCurrentPassword_ShouldReturnValidationFailure(string? currentPassword)
    {
        // Arrange
        ModifyPasswordInformation information = new(currentPassword!, ValidatorTestCaseSources.ValidPasswordSource().First());

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(typeof(ValidatorTestCaseSources), nameof(ValidatorTestCaseSources.InvalidPasswordSource))]
    public async Task Validate_WithInvalidNewPassword_ShouldReturnValidationFailure(string? newPassword)
    {
        // Arrange
        ModifyPasswordInformation information = new(ValidatorTestCaseSources.ValidPasswordSource().First(), newPassword!);

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test, TestCaseSource(nameof(ValidPasswordPairSource))]
    public async Task Validate_WithValidAndNonEqualPasswordPair_ShouldReturnValidationSuccess(ModifyPasswordInformation information)
    {
        // Arrange

        // Act
        ValidationResult validationResult = await _validator.ValidateAsync(information);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    private static IEnumerable<ModifyPasswordInformation> EqualPasswordPairsSource()
    {
        foreach (string password in ValidatorTestCaseSources.ValidPasswordSource())
        {
            yield return new ModifyPasswordInformation(password, password);
        }
    }

    private static IEnumerable<ModifyPasswordInformation> ValidPasswordPairSource()
    {
        using IEnumerator<string> passwordEnumerator1 = ValidatorTestCaseSources.ValidPasswordSource().Reverse().GetEnumerator();
        using IEnumerator<string> passwordEnumerator2 = ValidatorTestCaseSources.ValidPasswordSource().GetEnumerator();

        while (passwordEnumerator1.MoveNext() && passwordEnumerator2.MoveNext())
        {
            if (passwordEnumerator1.Current != passwordEnumerator2.Current)
            {
                yield return new ModifyPasswordInformation(passwordEnumerator1.Current, passwordEnumerator2.Current);
            }
        }
    }
}
