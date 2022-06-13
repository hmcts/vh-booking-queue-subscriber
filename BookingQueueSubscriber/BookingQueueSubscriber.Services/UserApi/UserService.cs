using System;
using System.Net;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingsApi.Contract.Requests;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;
using UserNotificationQueueSubscriber.Services;

namespace BookingQueueSubscriber.Services.UserApi
{
    public interface IUserService
    {
        //Task<User> CreateNewUserForParticipant(ParticipantRequest participant);
        Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser);
    }

    public class UserService : IUserService
    {
        private readonly IUserApiClient _userApiClient;

        public const string Representative = "Representative";
        public const string Joh = "Judicial Office Holder";
        public const string External = "External";
        public const string Internal = "Internal";
        public const string VirtualRoomProfessionalUser = "VirtualRoomProfessionalUser";
        public const string JudicialOfficeHolder = "JudicialOfficeHolder";
        public const string StaffMember = "Staff Member";
        public UserService(IUserApiClient userApiClient)
        {
            _userApiClient = userApiClient;
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



        public async Task AssignParticipantToGroup(string username, string userRole)
        {
            switch (userRole)
            {
                case "Representative":
                    await AddGroup(username, External);
                    await AddGroup(username, VirtualRoomProfessionalUser);
                    break;
                case "Joh":
                    await AddGroup(username, External);
                    await AddGroup(username, JudicialOfficeHolder);
                    break;
                case "StaffMember":
                    await AddGroup(username, Internal);
                    await AddGroup(username, StaffMember);
                    break;
                default:
                    await AddGroup(username, External);
                    break;
            }
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
        private async Task AddGroup(string username, string groupName)
        {
            try
            {
                var addUserToGroupRequest = new AddUserToGroupRequest
                {
                    UserId = username,
                    GroupName = groupName
                };
                await _userApiClient.AddUserToGroupAsync(addUserToGroupRequest);
                //_logger.LogDebug("{username} to group {group}.", username, addUserToGroupRequest.GroupName);
            }
            catch (UserApiException e)
            {
                //_logger.LogError(e,
                //    $"Failed to add user {username} to {groupName} in User API. " +
                //    $"Status Code {e.StatusCode} - Message {e.Message}");
                throw;
            }
        }
    }
}
