using System.Net;
using BookingQueueSubscriber.Services.UserApi;
using Microsoft.Extensions.Logging;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.UserServiceTests;

public class CreateNewUserForParticipantTests
{
    private Mock<IUserApiClient> _userApiClientMock;
    private IUserService _userService;

    [SetUp]
    public void SetUp()
    {
        var loggerMock = new Mock<ILogger<UserService>>();
        _userApiClientMock = new Mock<IUserApiClient>();
        _userService = new UserService(_userApiClientMock.Object, loggerMock.Object);
    }

    [Test]
    public async Task Should_create_new_user_for_participant()
    {
        // Arrange
        const string firstName = "FirstName";
        const string lastName = "LastName";
        const string contactEmail = "email@email.com";
        const string userId = "123";
        const string username = "Username";
        const string oneTimePassword = "OneTimePassword";

        _userApiClientMock.Setup(x => x.GetUserByEmailAsync(contactEmail))
            .ThrowsAsync(new UserApiException("NotFound", 
                (int)HttpStatusCode.NotFound, 
                "NotFound", 
                new Dictionary<string, IEnumerable<string>>(), 
                new Exception()));
        _userApiClientMock.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(new NewUserResponse
        {
            UserId = userId,
            OneTimePassword = oneTimePassword,
            Username = username
        });

        // Act
        var user = await _userService.CreateNewUserForParticipantAsync(firstName, lastName, contactEmail, false);

        // Assert
        user.Should().NotBeNull();
        user.UserId.Should().Be(userId);
        user.UserName.Should().Be(username);
        user.Password.Should().Be(oneTimePassword);
        user.ContactEmail.Should().Be(contactEmail);
    }

    [Test]
    public async Task Should_return_existing_user_when_already_exists()
    {
        // Arrange
        const string firstName = "FirstName";
        const string lastName = "LastName";
        const string contactEmail = "email@email.com";
        const string userId = "123";
        const string username = "Username";
        
        _userApiClientMock.Setup(x => x.GetUserByEmailAsync(contactEmail)).ReturnsAsync(new UserProfile
        {
            UserId = userId,
            UserName = username
        });
        
        // Act
        var user = await _userService.CreateNewUserForParticipantAsync(firstName, lastName, contactEmail, false);
        
        // Assert
        user.Should().NotBeNull();
        user.UserName.Should().Be(username);
        user.ContactEmail.Should().Be(contactEmail);
    }
    
    [Test]
    public async Task Should_return_user_after_retry_to_get_user_by_email_succeeded()
    {
        // Arrange
        const string firstName = "FirstName";
        const string lastName = "LastName";
        const string contactEmail = "email@email.com";
        const string userId = "123";
        const string username = "Username";
        
        _userApiClientMock.SetupSequence(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserProfile)null) // First call returns null
            .ReturnsAsync(new UserProfile // Second call returns a value
            {
                UserId = userId,
                UserName = username
            });
        _userApiClientMock.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>()))
            .ThrowsAsync(new UserApiException("Conflict", 
                (int)HttpStatusCode.Conflict, 
                "Conflict", 
                new Dictionary<string, IEnumerable<string>>(), 
                new Exception()));
        
        // Act
        var user = await _userService.CreateNewUserForParticipantAsync(firstName, lastName, contactEmail, false);
        
        // Assert
        user.Should().NotBeNull();
        user.UserName.Should().Be(username);
        user.ContactEmail.Should().Be(contactEmail);
        _userApiClientMock.Verify(x => x.GetUserByEmailAsync(contactEmail), Times.Exactly(2));
    }

    [Test]
    public async Task Should_return_null_after_final_retry_to_get_user_by_email_failed()
    {
        // Arrange
        const string firstName = "FirstName";
        const string lastName = "LastName";
        const string contactEmail = "email@email.com";
        
        _userApiClientMock.Setup(x => x.GetUserByEmailAsync(contactEmail)).ReturnsAsync((UserProfile)null);
        _userApiClientMock.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>()))
            .ThrowsAsync(new UserApiException("Conflict", 
                (int)HttpStatusCode.Conflict, 
                "Conflict", 
                new Dictionary<string, IEnumerable<string>>(), 
                new Exception()));
        
        // Act
        var user = await _userService.CreateNewUserForParticipantAsync(firstName, lastName, contactEmail, false);
        
        // Assert
        user.Should().BeNull();
        _userApiClientMock.Verify(x => x.GetUserByEmailAsync(contactEmail), Times.Exactly(2));
    }

    [Test]
    public async Task Should_return_null_when_create_new_user_throws_internal_server_error_exception()
    {
        // Arrange
        const string firstName = "FirstName";
        const string lastName = "LastName";
        const string contactEmail = "email@email.com";
        
        _userApiClientMock.Setup(x => x.GetUserByEmailAsync(contactEmail)).ReturnsAsync((UserProfile)null);
        _userApiClientMock.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>()))
            .ThrowsAsync(new UserApiException("Exception", 
                (int)HttpStatusCode.InternalServerError, 
                "Internal Server Error", 
                new Dictionary<string, IEnumerable<string>>(), 
                new Exception()));
        
        // Act
        var user = await _userService.CreateNewUserForParticipantAsync(firstName, lastName, contactEmail, false);
        
        // Assert
        user.Should().BeNull();
        _userApiClientMock.Verify(x => x.GetUserByEmailAsync(contactEmail), Times.Once);
    }

    [Test]
    public Task Should_throw_exception_when_get_user_by_email_throws_internal_server_error()
    {
        // Arrange
        const string contactEmail = "email@email.com";
        
        _userApiClientMock.Setup(x => x.GetUserByEmailAsync(contactEmail))
            .ThrowsAsync(new UserApiException("InternalServerError", 
                (int)HttpStatusCode.InternalServerError, 
                "InternalServerError", 
                new Dictionary<string, IEnumerable<string>>(), 
                new Exception()));
        
        // Act & Assert
        Assert.ThrowsAsync<UserApiException>(() => _userService.CreateNewUserForParticipantAsync("FirstName", "LastName", contactEmail, false));
        return Task.CompletedTask;
    }
}