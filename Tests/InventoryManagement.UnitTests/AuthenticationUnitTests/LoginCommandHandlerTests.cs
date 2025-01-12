using FluentAssertions;

using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Authentication.Errors;
using InventoryManagement.Api.Features.Authentication.Login;
using InventoryManagement.Api.Features.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Moq;

namespace InventoryManagement.UnitTests.AuthenticationUnitTests;

[TestFixture]
public class LoginCommandHandlerTests
{
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<SignInManager<User>> _signInManagerMock;
    private Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private Mock<IValidator<LoginInformation>> _validatorMock;
    private LoginCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userManagerMock = MockHelpers.MockUserManager<User>();
        _signInManagerMock = MockHelpers.MockSignInManager(_userManagerMock.Object);
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();
        _validatorMock = new Mock<IValidator<LoginInformation>>();
        _handler = new LoginCommandHandler(_userManagerMock.Object, _signInManagerMock.Object, _loggerMock.Object, _validatorMock.Object);
    }

    [Test]
    public async Task Handle_ValidLogin_ReturnsSuccess()
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        User testUser = User.Create(@"testUser@test.to", @"testuser");
        LoginInformation sampleLoginInformation = new(@"testUser@test.to", @"testpassword123");

        _validatorMock.Setup(v => v.ValidateAsync(sampleLoginInformation, tokenSource.Token))
            .Returns(Task.FromResult(new ValidationResult()));

        _userManagerMock.Setup(um => um.FindByEmailAsync(sampleLoginInformation.EmailAddress))
            .Returns(Task.FromResult<User?>(testUser));

        _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(testUser, sampleLoginInformation.Password, true, false))
            .Returns(Task.FromResult(SignInResult.Success));

        // Act
        Result result = await _handler.Handle(sampleLoginInformation, tokenSource.Token);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _validatorMock.Verify(mock => mock.ValidateAsync(
            It.Is<LoginInformation>(info => info == sampleLoginInformation),
            It.Is<CancellationToken>(ct => ct == tokenSource.Token)),
            Times.Once);

        _userManagerMock.Verify(mock => mock.FindByEmailAsync(
            It.Is<string>(email => email == sampleLoginInformation.EmailAddress)),
            Times.Once);

        _signInManagerMock.Verify(mock => mock.PasswordSignInAsync(
            It.Is<User>(user => user == testUser),
            It.Is<string>(pw => pw == sampleLoginInformation.Password),
            It.Is<bool>(isPersistence => isPersistence == true),
            It.Is<bool>(lockout => lockout == false)),
            Times.Once);
    }

    [Test]
    public async Task Handle_InvalidLoginInformation_ReturnsValidationError()
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        LoginInformation sampleLoginInformation = new(@"testUser@test.to", @"testpassword123");
        List<ValidationFailure> validationErrors = [new ValidationFailure("Email", "Email address is required")];

        _validatorMock.Setup(v => v.ValidateAsync(sampleLoginInformation, tokenSource.Token))
            .Returns(Task.FromResult(new ValidationResult(validationErrors)));

        // Act
        Result result = await _handler.Handle(sampleLoginInformation, tokenSource.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.GetType() == typeof(InvalidDataError));

        _validatorMock.Verify(mock => mock.ValidateAsync(
            It.Is<LoginInformation>(info => info == sampleLoginInformation),
            It.Is<CancellationToken>(ct => ct == tokenSource.Token)),
            Times.Once);

        _userManagerMock.Verify(mock => mock.FindByEmailAsync(
            It.IsAny<string>()),
            Times.Never);

        _signInManagerMock.Verify(mock => mock.PasswordSignInAsync(
            It.IsAny<User>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_UserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        LoginInformation sampleLoginInformation = new(@"testUser@test.to", @"testpassword123");

        _validatorMock.Setup(v => v.ValidateAsync(sampleLoginInformation, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new ValidationResult()));
        _userManagerMock.Setup(um => um.FindByEmailAsync(sampleLoginInformation.EmailAddress))
            .Returns(Task.FromResult<User?>(null));

        // Act
        Result result = await _handler.Handle(sampleLoginInformation, tokenSource.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.GetType() == typeof(NotFoundError));

        _validatorMock.Verify(mock => mock.ValidateAsync(
            It.Is<LoginInformation>(info => info == sampleLoginInformation),
            It.Is<CancellationToken>(ct => ct == tokenSource.Token)),
            Times.Once);

        _userManagerMock.Verify(mock => mock.FindByEmailAsync(
            It.Is<string>(email => email == sampleLoginInformation.EmailAddress)),
            Times.Once);

        _signInManagerMock.Verify(mock => mock.PasswordSignInAsync(
            It.IsAny<User>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_IncorrectPassword_ReturnsIncorrectPasswordError()
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        User testUser = User.Create(@"testUser@test.to", @"testuser");
        LoginInformation sampleLoginInformation = new(@"testUser@test.to", @"testpassword123");

        _validatorMock.Setup(v => v.ValidateAsync(sampleLoginInformation, tokenSource.Token))
            .Returns(Task.FromResult(new ValidationResult()));

        _userManagerMock.Setup(um => um.FindByEmailAsync(sampleLoginInformation.EmailAddress))
            .Returns(Task.FromResult<User?>(testUser));

        _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(testUser, sampleLoginInformation.Password, true, false))
            .Returns(Task.FromResult(SignInResult.Failed));

        // Act
        Result result = await _handler.Handle(sampleLoginInformation, tokenSource.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.GetType() == typeof(IncorrectPasswordError));

        _validatorMock.Verify(mock => mock.ValidateAsync(
            It.Is<LoginInformation>(info => info == sampleLoginInformation),
            It.Is<CancellationToken>(ct => ct == tokenSource.Token)),
            Times.Once);

        _userManagerMock.Verify(mock => mock.FindByEmailAsync(
            It.Is<string>(email => email == sampleLoginInformation.EmailAddress)),
            Times.Once);

        _signInManagerMock.Verify(mock => mock.PasswordSignInAsync(
            It.Is<User>(user => user == testUser),
            It.Is<string>(pw => pw == sampleLoginInformation.Password),
            It.Is<bool>(isPersistence => isPersistence == true),
            It.Is<bool>(lockout => lockout == false)),
            Times.Once);
    }
}