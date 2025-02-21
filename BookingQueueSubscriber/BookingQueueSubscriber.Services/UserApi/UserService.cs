using System.Net;
using BookingQueueSubscriber.Common.Extensions;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.UserApi
{
    public interface IUserService
    {
        Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser);
        Task AssignUserToGroup(string userId, string userRole);
        Task UpdateUserContactEmail(string existingContactEmail, string newContactEmail);
    }

    public class UserService : IUserService
    {
        private readonly IUserApiClient _userApiClient;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserApiClient userApiClient, ILogger<UserService> logger)
        {
            _userApiClient = userApiClient;
            _logger = logger;
        }

        public async Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail,
            bool isTestUser)
        {
            var userProfile = await GetUserByContactEmail(contactEmail);
            if (userProfile == null)
            {
                _logger.LogInformation("User with contact email {ContactEmail} does not exist. Creating an account.",
                    contactEmail);

                try
                {
                    // create the user in AD.
                    var newUser = await CreateNewUserInAd(firstname, lastname, contactEmail, isTestUser);
                    
                    return new User
                    {
                        UserId = newUser.UserId,
                        UserName = newUser.Username,
                        Password = newUser.OneTimePassword,
                        ContactEmail = contactEmail
                    };
                }
                catch (UserApiException e)
                {
                    if (e.StatusCode == (int) HttpStatusCode.Conflict)
                    {
                        Thread.Sleep(1000);
                        userProfile = await GetUserByContactEmail(contactEmail);
                        if (userProfile == null)
                        {
                            _logger.LogError(e,
                                "User with contact email {ContactEmail} does not exist. Creating an account. Second try.",
                                contactEmail);
                            return null;
                        }
                    }
                    else
                    {
                        _logger.LogError(e,
                            "User with contact email {ContactEmail} does not exist. Creating an account. Second try.",
                            contactEmail);
                        return null;
                    }
                }
            }

            return new User
            {
                UserId = userProfile.UserId,
                UserName = userProfile.UserName,
                ContactEmail = contactEmail
            };

        }

        public async Task AssignUserToGroup(string userId, string userRole)
        {
            _logger.LogInformation("Assigning the user to the group based on the user role {UserRole}", userRole);
            switch (userRole)
            {
                case "Representative":
                    await AddGroup(userId, UserGroup.External);
                    await AddGroup(userId, UserGroup.VirtualRoomProfessionalUser);
                    break;
                case "Judicial Office Holder":
                    await AddGroup(userId, UserGroup.External);
                    await AddGroup(userId, UserGroup.JudicialOfficeHolder);
                    break;
                case "StaffMember":
                    await AddGroup(userId, UserGroup.Internal);
                    await AddGroup(userId, UserGroup.StaffMember);
                    break;
                default:
                    await AddGroup(userId, UserGroup.External);
                    break;
            }
        }
        
        public async Task UpdateUserContactEmail(string existingContactEmail, string newContactEmail)
        {
            var userProfile = await GetUserByContactEmail(existingContactEmail);
            var userId = Guid.Parse(userProfile.UserId);
            var request = new UpdateUserAccountRequest
            {
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                ContactEmail = newContactEmail
            };
            await _userApiClient.UpdateUserAccountAsync(userId, request);
        }
        
        private async Task<NewUserResponse> CreateNewUserInAd(string firstname, string lastname, string contactEmail, bool isTestUser)
        {
            const string blank = " ";
            _logger.LogInformation("Attempting to create an AD user with contact email {ContactEmail}.", contactEmail);
            var createUserRequest = new CreateUserRequest
            {
                FirstName = firstname?.Replace(blank, string.Empty),
                LastName = lastname?.Replace(blank, string.Empty),
                RecoveryEmail = contactEmail,
                IsTestUser = isTestUser
            };

            var newUserResponse = await _userApiClient.CreateUserAsync(createUserRequest);
            _logger.LogDebug("Successfully created an AD user with contact email {ContactEmail}.", contactEmail);
            return newUserResponse;
        }

        private async Task<UserProfile> GetUserByContactEmail(string emailAddress)
        {
            _logger.LogInformation("Attempt to get username by contact email {ContactEmail}.", emailAddress);
            
            // Remove diacritics from the email address
            // We have to do this here rather than in user api as diacritic characters do not get url-encoded by the user api client
            var sanitisedEmailAddress = emailAddress.ReplaceDiacriticCharacters();
            try
            {
                var user = await _userApiClient.GetUserByEmailAsync(sanitisedEmailAddress);
                _logger.LogInformation("User with contact email {ContactEmail} found.", sanitisedEmailAddress);
                return user;
            }
            catch (UserApiException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    _logger.LogWarning(e, "User with contact email {ContactEmail} not found.", sanitisedEmailAddress);
                    return null;
                }
                
                throw;
            }
        }
        private async Task AddGroup(string userId, string groupName)
        {
            var addUserToGroupRequest = new AddUserToGroupRequest
            {
                UserId = userId,
                GroupName = groupName
            };
            await _userApiClient.AddUserToGroupAsync(addUserToGroupRequest);
            _logger.LogDebug("{Username} to group {Group}.", userId, addUserToGroupRequest.GroupName);
        }
    }
}
