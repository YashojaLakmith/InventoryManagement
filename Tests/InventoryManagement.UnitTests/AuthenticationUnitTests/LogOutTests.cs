using FluentAssertions;

using InventoryManagement.Api.Features.Authentication.Logout;
using InventoryManagement.Api.Features.Users;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

using Moq;

namespace InventoryManagement.UnitTests.AuthenticationUnitTests;

[TestFixture]
public class LogOutTests
{
    private Mock<SignInManager<User>> _signInManagerMock;

    [SetUp]
    public void Setup()
    {
        _signInManagerMock = MockHelpers.MockSignInManager<User>();
    }

    [Test]
    public async Task LogOutAsync_ShouldCallSignOutInSignInManagerAndReturnOk()
    {
        // Arrange
        _signInManagerMock.Setup(sm => sm.SignOutAsync())
            .Returns(Task.CompletedTask);

        // Act
        IResult result = await LogoutEndpoint.LogOutAsync(_signInManagerMock.Object);

        // Assert
        result.Should().BeOfType<Ok>();
        _signInManagerMock.Verify(sm => sm.SignOutAsync(), Times.Once);
    }
}
