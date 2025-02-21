using BookingQueueSubscriber.Common.Extensions;
using BookingQueueSubscriber.Services.UserApi;
using Microsoft.Extensions.Logging;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.UserServiceTests
{
    public class UpdateUserContactEmailTests
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
        public async Task Should_update_user_contact_email()
        {
            // Arrange
            const string existingContactEmail = "email@email.com";
            const string newContactEmail = "newEmail@email.com";
            var user = new UserProfile
            {
                UserId = Guid.NewGuid().ToString(),
                FirstName = "FirstName",
                LastName = "LastName",
                Email = existingContactEmail
            };
            _userApiClientMock.Setup(x => x.GetUserByEmailAsync(existingContactEmail)).ReturnsAsync(user);
            
            // Act
            await _userService.UpdateUserContactEmail(existingContactEmail, newContactEmail);

            // Assert
            _userApiClientMock.Verify(x => x.GetUserByEmailAsync(
                It.Is<string>(e => e == existingContactEmail)
            ), Times.Once);
            
            _userApiClientMock.Verify(x => x.UpdateUserAccountAsync(
                It.IsAny<Guid>(), 
                It.Is<UpdateUserAccountRequest>(r => 
                    r.FirstName == user.FirstName &&
                    r.LastName == user.LastName &&
                    r.ContactEmail == newContactEmail
                )
            ), Times.Once);
        }

        [Test]
        public async Task Should_update_user_contact_email_containing_diacritic_characters()
        {
            // Arrange
            const string existingContactEmail = "Áá@créâtïvéàççénts.com";
            const string newContactEmail = "çÁá@créâtïvéàççénts.com";
            var existingContactEmailWithoutDiacritics = existingContactEmail.RemoveDiacriticCharacters();
            var user = new UserProfile
            {
                UserId = Guid.NewGuid().ToString(),
                FirstName = "FirstName",
                LastName = "LastName",
                Email = existingContactEmailWithoutDiacritics
            };
            _userApiClientMock.Setup(x => x.GetUserByEmailAsync(existingContactEmailWithoutDiacritics)).ReturnsAsync(user);
            
            // Act
            await _userService.UpdateUserContactEmail(existingContactEmail, newContactEmail);

            // Assert
            _userApiClientMock.Verify(x => x.GetUserByEmailAsync(
                It.Is<string>(e => e == existingContactEmailWithoutDiacritics)
            ), Times.Once);
                
            _userApiClientMock.Verify(x => x.UpdateUserAccountAsync(
                It.IsAny<Guid>(), 
                It.Is<UpdateUserAccountRequest>(r => 
                    r.FirstName == user.FirstName &&
                    r.LastName == user.LastName &&
                    r.ContactEmail == newContactEmail
                )
            ), Times.Once);
        }
    }
}
