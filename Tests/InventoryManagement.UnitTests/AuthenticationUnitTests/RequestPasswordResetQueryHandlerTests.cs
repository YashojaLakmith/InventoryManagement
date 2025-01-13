using FluentAssertions;

using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Features.Authentication.RequestPasswordReset;
using InventoryManagement.Api.Features.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Moq;

namespace InventoryManagement.UnitTests.AuthenticationUnitTests;

[TestFixture]
public class RequestPasswordResetQueryHandlerTests
{
    private Mock<IValidator<RequestPasswordResetQuery>> _validatorMock;
    private Mock<ILogger<RequestPasswordResetQueryHandler>> _loggerMock;
    private Mock<IEmailSender<User>> _emailSenderMock;
    private Mock<UserManager<User>> _userManagerMock;
    private RequestPasswordResetQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _validatorMock = new Mock<IValidator<RequestPasswordResetQuery>>();
        _loggerMock = new Mock<ILogger<RequestPasswordResetQueryHandler>>();
        _emailSenderMock = new Mock<IEmailSender<User>>();
        _userManagerMock = MockHelpers.MockUserManager<User>();
        _handler = new RequestPasswordResetQueryHandler(_emailSenderMock.Object, _userManagerMock.Object, _loggerMock.Object, _validatorMock.Object);
    }

    [Test]
    public async Task Handle_OnValidRequestData_SendCreatedTokenAndReturnOk()
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        RequestPasswordResetQuery sampleQuery = new(@"testmail@test.mail");
        User sampleUser = User.Create(sampleQuery.EmailAddress, @"testUser");
        const string sampleToken = @"12345";

        _validatorMock.Setup(v => v.ValidateAsync(sampleQuery, tokenSource.Token))
            .Returns(Task.FromResult(new ValidationResult()));

        _userManagerMock.Setup(um => um.FindByEmailAsync(sampleQuery.EmailAddress))
            .Returns(Task.FromResult<User?>(sampleUser));

        _userManagerMock.Setup(um => um.GeneratePasswordResetTokenAsync(sampleUser))
            .Returns(Task.FromResult(sampleToken));

        _emailSenderMock.Setup(es => es.SendPasswordResetCodeAsync(sampleUser, sampleQuery.EmailAddress, sampleToken))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _handler.Handle(sampleQuery, tokenSource.Token);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _validatorMock.Verify(v => v.ValidateAsync(
            It.Is<RequestPasswordResetQuery>(query => query == sampleQuery),
            It.Is<CancellationToken>(ct => ct == tokenSource.Token)),
            Times.Once);

        _userManagerMock.Verify(um => um.FindByEmailAsync(
            It.Is<string>(email => email == sampleQuery.EmailAddress)),
            Times.Once);

        _userManagerMock.Verify(um => um.GeneratePasswordResetTokenAsync(
            It.Is<User>(user => user == sampleUser)),
            Times.Once);

        _emailSenderMock.Verify(es => es.SendPasswordResetCodeAsync(
            It.Is<User>(user => user == sampleUser),
            It.Is<string>(email => email == sampleUser.Email && email == sampleQuery.EmailAddress),
            It.Is<string>(code => code == sampleToken)),
            Times.Once);
    }

    [Test]
    public async Task Handle_OnInvalidRequestData_ReturnFailureResultWithInvalidDataError()
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        RequestPasswordResetQuery sampleQuery = new(@"");
        ValidationFailure sampleFailure = new(nameof(sampleQuery.EmailAddress), @"Email is invalid");

        _validatorMock.Setup(v => v.ValidateAsync(sampleQuery, tokenSource.Token))
            .Returns(Task.FromResult(new ValidationResult([sampleFailure])));

        // Act
        Result result = await _handler.Handle(sampleQuery, tokenSource.Token);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.GetType() == typeof(InvalidDataError));

        _validatorMock.Verify(v => v.ValidateAsync(
            It.Is<RequestPasswordResetQuery>(query => query == sampleQuery),
            It.Is<CancellationToken>(ct => ct == tokenSource.Token)),
            Times.Once);

        _userManagerMock.Verify(um => um.FindByEmailAsync(
            It.IsAny<string>()),
            Times.Never);

        _userManagerMock.Verify(um => um.GeneratePasswordResetTokenAsync(
            It.IsAny<User>()),
            Times.Never);

        _emailSenderMock.Verify(es => es.SendPasswordResetCodeAsync(
            It.IsAny<User>(),
            It.IsAny<string>(),
            It.IsAny<string>()),
            Times.Never);
    }
}
