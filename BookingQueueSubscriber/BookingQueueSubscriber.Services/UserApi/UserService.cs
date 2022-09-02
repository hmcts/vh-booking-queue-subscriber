using System;
using System.Net;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.Configuration;
using Microsoft.Extensions.Logging;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.UserApi
{
    public interface IUserService
    {
        Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser);
        Task AssignUserToGroup(string userId, string userRole);
    }

    public class UserService : IUserService
    {
        private readonly IUserApiClient _userApiClient;
        private readonly ILogger<UserService> _logger;
        private readonly IFeatureToggles _featureToggles;

        public const string Representative = "Representative";
        public const string Joh = "Judicial Office Holder";
        public const string External = "External";
        public const string Internal = "Internal";
        public const string VirtualRoomProfessionalUser = "VirtualRoomProfessionalUser";
        public const string JudicialOfficeHolder = "JudicialOfficeHolder";
        public const string StaffMember = "Staff Member";
        public const string SsprEnabled = "SSPR Enabled";
        
        public UserService(IUserApiClient userApiClient, ILogger<UserService> logger, IFeatureToggles featureToggles)
        {
            _userApiClient = userApiClient;
            _logger = logger;
            _featureToggles = featureToggles;
        }

        public async Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser)
        {
            var userProfile = await GetUserByContactEmail(contactEmail);
            if (userProfile == null)
            {
                _logger.LogInformation("User with contact email {contactEmail} does not exist. Creating an account.", contactEmail);
                // create the user in AD.
                var newUser = await CreateNewUserInAD(firstname, lastname, contactEmail, isTestUser);
                return new User
                {
                    UserId = newUser.UserId,
                    UserName = newUser.Username,
                    Password = newUser.OneTimePassword
                };
            }

            return new User
            {
                UserId = userProfile.UserId,
                UserName = userProfile.UserName
            };

        }

        public async Task AssignUserToGroup(string userId, string userRole)
        {
            _logger.LogInformation("Assigning the user to the group based on the userrole {userRole}", userRole);
            switch (userRole)
            {
                case "Representative":
                    await AddGroup(userId, External);
                    await AddGroup(userId, VirtualRoomProfessionalUser);
                    break;
                case "Judicial Office Holder":
                    await AddGroup(userId, External);
                    await AddGroup(userId, JudicialOfficeHolder);
                    break;
                case "StaffMember":
                    await AddGroup(userId, Internal);
                    await AddGroup(userId, StaffMember);
                    break;
                default:
                    await AddGroup(userId, External);
                    break;
            }
            if (_featureToggles.SsprToggle())
            {
                await AddGroup(userId, SsprEnabled);
            }
        }
        private async Task<NewUserResponse> CreateNewUserInAD(string firstname, string lastname, string contactEmail, bool isTestUser)
        {
            const string BLANK = " ";
            _logger.LogInformation("Attempting to create an AD user with contact email {contactEmail}.", contactEmail);
            var createUserRequest = new CreateUserRequest
            {
                FirstName = firstname?.Replace(BLANK, string.Empty),
                LastName = lastname?.Replace(BLANK, string.Empty),
                RecoveryEmail = contactEmail,
                IsTestUser = isTestUser
            };

            var newUserResponse = await _userApiClient.CreateUserAsync(createUserRequest);
            _logger.LogDebug("Successfully created an AD user with contact email {contactEmail}.", contactEmail);
            return newUserResponse;
        }

        private async Task<UserProfile> GetUserByContactEmail(string emailAddress)
        {
            _logger.LogInformation("Attempt to get username by contact email {contactEmail}.", emailAddress);
            try
            {
                var user = await _userApiClient.GetUserByEmailAsync(emailAddress);
                _logger.LogInformation("User with contact email {contactEmail} found.", emailAddress);
                return user;
            }
            catch (UserApiException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("User with contact email {contactEmail} not found.", emailAddress);
                    return null;
                }

                _logger.LogError(e, "Unhandled error getting a user with contact email {contactEmail}.", emailAddress);
                throw;
            }
        }
        private async Task AddGroup(string userId, string groupName)
        {
            try
            {
                var addUserToGroupRequest = new AddUserToGroupRequest
                {
                    UserId = userId,
                    GroupName = groupName
                };
                await _userApiClient.AddUserToGroupAsync(addUserToGroupRequest);
                _logger.LogDebug("{username} to group {group}.", userId, addUserToGroupRequest.GroupName);
            }
            catch (UserApiException e)
            {
                _logger.LogError(e,
                    $"Failed to add user {userId} to {groupName} in User API. " +
                    $"Status Code {e.StatusCode} - Message {e.Message}");
                throw;
            }
        }
    }
}
