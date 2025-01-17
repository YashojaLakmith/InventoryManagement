using FluentAssertions;

using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Authentication.ResetPassword;
using InventoryManagement.Api.Features.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Moq;

namespace InventoryManagement.UnitTests.AuthenticationUnitTests;

[TestFixture]
public class ResetPasswordCommandHandlerTests
{
    private Mock<IValidator<PasswordResetTokenData>> _validatorMock;
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<ILogger<PasswordResetCommandHandler>> _loggerMock;
    private PasswordResetCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _validatorMock = new Mock<IValidator<PasswordResetTokenData>>();
        _userManagerMock = MockHelpers.MockUserManager<User>();
        _loggerMock = new Mock<ILogger<PasswordResetCommandHandler>>();
        _handler = new PasswordResetCommandHandler(_validatorMock.Object, _userManagerMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithInvalidInput_ShouldReturnFailureReturnWithInvalidDataError()
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        PasswordResetTokenData sampleTokenData = new(@"ssgddf", @"dfhbfhdbf", @"jdfhdhddf");
        ValidationFailure validationFailure = new();

        _validatorMock.Setup(v => v.ValidateAsync(sampleTokenData, tokenSource.Token))
            .Returns(Task.FromResult(new ValidationResult([validationFailure])));

        // Act
        Result result = await _handler.Handle(sampleTokenData, tokenSource.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.GetType() == typeof(InvalidDataError));

        _validatorMock.Verify(v => v.ValidateAsync(
            It.Is<PasswordResetTokenData>(data => data == sampleTokenData),
            It.Is<CancellationToken>(ct => ct == tokenSource.Token)),
            Times.Once);

        _userManagerMock.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Never);
        _userManagerMock.Verify(um => um.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Handle_WithEmailOfNonExistingUser_ShouldReturnFailureResultWithNotFoundError()
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        PasswordResetTokenData sampleTokenData = new(@"ssgddf@test.mail", @"dfhbfhdbf", @"jdfhdhddf");

        _validatorMock.Setup(v => v.ValidateAsync(sampleTokenData, tokenSource.Token))
            .Returns(Task.FromResult(new ValidationResult()));

        _userManagerMock.Setup(um => um.FindByEmailAsync(sampleTokenData.EmailAddress))
            .Returns(Task.FromResult<User?>(null));

        // Act
        Result result = await _handler.Handle(sampleTokenData, tokenSource.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.GetType() == typeof(NotFoundError));

        _validatorMock.Verify(v => v.ValidateAsync(
            It.Is<PasswordResetTokenData>(data => data == sampleTokenData),
            It.Is<CancellationToken>(ct => ct == tokenSource.Token)),
            Times.Once);

        _userManagerMock.Verify(um => um.FindByEmailAsync(It.Is<string>(mail => mail == sampleTokenData.EmailAddress)), Times.Once);
        _userManagerMock.Verify(um => um.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Handle_WithInvalidOrExpiredToken_ShouldReturnFailureResultWithInvalidDataError()
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        PasswordResetTokenData sampleTokenData = new(@"ssgddf@test.mail", @"dfhbfhdbf", @"jdfhdhddf");
        User sampleUser = User.Create(sampleTokenData.EmailAddress, @"Name");

        _validatorMock.Setup(v => v.ValidateAsync(sampleTokenData, tokenSource.Token))
            .Returns(Task.FromResult(new ValidationResult()));

        _userManagerMock.Setup(um => um.FindByEmailAsync(sampleTokenData.EmailAddress))
            .Returns(Task.FromResult<User?>(sampleUser));

        _userManagerMock.Setup(um => um.ResetPasswordAsync(sampleUser, sampleTokenData.ResetToken, sampleTokenData.NewPassword))
            .Returns(Task.FromResult(IdentityResult.Failed()));

        // Act
        Result result = await _handler.Handle(sampleTokenData, tokenSource.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.GetType() == typeof(InvalidDataError));

        _validatorMock.Verify(v => v.ValidateAsync(
            It.Is<PasswordResetTokenData>(data => data == sampleTokenData),
            It.Is<CancellationToken>(ct => ct == tokenSource.Token)),
            Times.Once);

        _userManagerMock.Verify(um => um.FindByEmailAsync(
            It.Is<string>(mail => mail == sampleTokenData.EmailAddress)),
            Times.Once);

        _userManagerMock.Verify(um => um.ResetPasswordAsync(
            It.Is<User>(user => user == sampleUser),
            It.Is<string>(token => token == sampleTokenData.ResetToken),
            It.Is<string>(pw => pw == sampleTokenData.NewPassword)),
            Times.Once);
    }

    [Test]
    public async Task Handle_WithValidDataAndValidToken_ShouldRetunSuccessResult()
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        PasswordResetTokenData sampleTokenData = new(@"ssgddf@test.mail", @"dfhbfhdbf", @"jdfhdhddf");
        User sampleUser = User.Create(sampleTokenData.EmailAddress, @"Name");

        _validatorMock.Setup(v => v.ValidateAsync(sampleTokenData, tokenSource.Token))
            .Returns(Task.FromResult(new ValidationResult()));

        _userManagerMock.Setup(um => um.FindByEmailAsync(sampleTokenData.EmailAddress))
            .Returns(Task.FromResult<User?>(sampleUser));

        _userManagerMock.Setup(um => um.ResetPasswordAsync(sampleUser, sampleTokenData.ResetToken, sampleTokenData.NewPassword))
            .Returns(Task.FromResult(IdentityResult.Success));

        // Act
        Result result = await _handler.Handle(sampleTokenData, tokenSource.Token);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _validatorMock.Verify(v => v.ValidateAsync(
            It.Is<PasswordResetTokenData>(data => data == sampleTokenData),
            It.Is<CancellationToken>(ct => ct == tokenSource.Token)),
            Times.Once);

        _userManagerMock.Verify(um => um.FindByEmailAsync(
            It.Is<string>(mail => mail == sampleTokenData.EmailAddress)),
            Times.Once);

        _userManagerMock.Verify(um => um.ResetPasswordAsync(
            It.Is<User>(user => user == sampleUser),
            It.Is<string>(token => token == sampleTokenData.ResetToken),
            It.Is<string>(pw => pw == sampleTokenData.NewPassword)),
            Times.Once);
    }
}
