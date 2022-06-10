using System;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;
using UserNotificationQueueSubscriber.Services;

namespace BookingQueueSubscriber.Services.UserApi
{
    public interface IUserService
    {
        Task<User> CreateNewUserForParticipant(ParticipantRequest participant);
        Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser);
    }

    public class UserService : IUserService
    {
        private readonly IUserApiClient _userApiClient;
        public async Task<User> CreateNewUserForParticipant(ParticipantRequest participant)
        {
            // create user in AD if users email does not exist in AD.
            //_logger.LogDebug("Checking for username with contact email {contactEmail}.", participant.ContactEmail);
            var userProfile = await GetUserByContactEmail(participant.ContactEmail);
            if (userProfile == null)
            {
                //_logger.LogDebug("User with contact email {contactEmail} does not exist. Creating an account.", participant.ContactEmail);
                // create the user in AD.
                var newUser = await CreateNewUserInAD(participant);
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

        public async Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser)
        {
            var userProfile = await GetUserByContactEmail(contactEmail);
            if (userProfile == null)
            {
                //_logger.LogDebug("User with contact email {contactEmail} does not exist. Creating an account.", participant.ContactEmail);
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


        private async Task<NewUserResponse> CreateNewUserInAD(ParticipantRequest participant)
        {
            const string BLANK = " ";
            //_logger.LogDebug("Attempting to create an AD user with contact email {contactEmail}.", participant.ContactEmail);
            var createUserRequest = new CreateUserRequest
            {
                FirstName = participant.FirstName?.Replace(BLANK, string.Empty),
                LastName = participant.LastName?.Replace(BLANK, string.Empty),
                RecoveryEmail = participant.ContactEmail,
                IsTestUser = false
            };

            var newUserResponse = await _userApiClient.CreateUserAsync(createUserRequest);
            //_logger.LogDebug("Successfully created an AD user with contact email {contactEmail}.", participant.ContactEmail);
            participant.Username = newUserResponse.Username;
            return newUserResponse;
        }
        private async Task<NewUserResponse> CreateNewUserInAD(string firstname, string lastname, string contactEmail, bool isTestUser)
        {
            const string BLANK = " ";
            //_logger.LogDebug("Attempting to create an AD user with contact email {contactEmail}.", participant.ContactEmail);
            var createUserRequest = new CreateUserRequest
            {
                FirstName = firstname?.Replace(BLANK, string.Empty),
                LastName = lastname?.Replace(BLANK, string.Empty),
                RecoveryEmail = contactEmail,
                IsTestUser = isTestUser
            };

            var newUserResponse = await _userApiClient.CreateUserAsync(createUserRequest);
            //_logger.LogDebug("Successfully created an AD user with contact email {contactEmail}.", participant.ContactEmail);
            //participant.Username = newUserResponse.Username;
            return newUserResponse;
        }

        private async Task<bool> CheckUsernameExistsInAdAsync(string username)
        {
            try
            {
                //_logger.LogDebug("Attempting to check if {username} exists in AD", username);
                var person = await _userApiClient.GetUserByAdUserNameAsync(username);
                Enum.TryParse<UserRoleType>(person.UserRole, out var userRoleResult);
                if (userRoleResult == UserRoleType.Judge || userRoleResult == UserRoleType.VhOfficer)
                {
                    var e = new UserServiceException
                    {
                        Reason = $"Unable to delete account with role {userRoleResult}"
                    };
                    //_logger.LogError(e, "Not allowed to delete {username}", username);
                    throw e;
                }

                //_logger.LogDebugk("{username} exists in AD", username);
                return true;
            }
            catch (UserApiException e)
            {
                //_logger.LogError(e, "Failed to get user {username} in User API. Status Code {StatusCode} - Message {Message}",
                //    username, e.StatusCode, e.Response);
                //if (e.StatusCode == (int)HttpStatusCode.NotFound)
                //{
                //    _logger.LogWarning(e, "{username} not found. Status Code {StatusCode} - Message {Message}",
                //        username, e.StatusCode, e.Response);
                //    return false;
                //}

                throw;
            }
        }

        private async Task<UserProfile> GetUserByContactEmail(string emailAddress)
        {
            //_logger.LogDebug("Attempt to get username by contact email {contactEmail}.", emailAddress);
            try
            {
                var user = await _userApiClient.GetUserByEmailAsync(emailAddress);
                //_logger.LogDebug("User with contact email {contactEmail} found.", emailAddress);
                return user;
            }
            catch (UserApiException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    //_logger.LogWarning("User with contact email {contactEmail} not found.", emailAddress);
                    return null;
                }

                //_logger.LogError(e, "Unhandled error getting a user with contact email {contactEmail}.", emailAddress);
                throw;
            }
        }

    }
}
