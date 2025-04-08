using System.Net;
using BookingQueueSubscriber.Common.Logging;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.UserApi;

public interface IUserService
{
    Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser);
    Task AssignUserToGroup(string userId, string userRole);
    Task UpdateUserContactEmail(string existingContactEmail, string newContactEmail);
}

public class UserService(IUserApiClient userApiClient, ILogger<UserService> logger) : IUserService
{
    public async Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser)
    {
        var userProfile = await GetUserByContactEmail(contactEmail);
        if (userProfile != null)
            return new User
            {
                UserId = userProfile.UserId,
                UserName = userProfile.UserName,
                ContactEmail = contactEmail
            };
        
        logger.UserDoesNotExist(contactEmail);
        
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
                    logger.CreateUserSecondTryError(e, contactEmail);
                    return null;
                }
            }
            else
            {
                logger.CreateUserSecondTryError(e, contactEmail);
                return null;
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
        logger.AssignUserToGroup(userRole);
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
        await userApiClient.UpdateUserAccountAsync(userId, request);
    }
        
    private async Task<NewUserResponse> CreateNewUserInAd(string firstname, string lastname, string contactEmail, bool isTestUser)
    {
        const string blank = " ";
        logger.CreatingAdUser(contactEmail);
        var createUserRequest = new CreateUserRequest
        {
            FirstName = firstname?.Replace(blank, string.Empty),
            LastName = lastname?.Replace(blank, string.Empty),
            RecoveryEmail = contactEmail,
            IsTestUser = isTestUser
        };

        var newUserResponse = await userApiClient.CreateUserAsync(createUserRequest);
        logger.CreatedAdUser(contactEmail);
        return newUserResponse;
    }

    private async Task<UserProfile> GetUserByContactEmail(string emailAddress)
    {
        logger.GettingUserByEmail(emailAddress);
        try
        {
            var user = await userApiClient.GetUserByEmailAsync(emailAddress);
            logger.UserFoundByEmail(emailAddress);
            return user;
        }
        catch (UserApiException e)
        {
            if (e.StatusCode == (int)HttpStatusCode.NotFound)
            {
                logger.UserNotFoundWarning(e, emailAddress);
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
        await userApiClient.AddUserToGroupAsync(addUserToGroupRequest);
        logger.AddedUserToGroup(addUserToGroupRequest.GroupName, userId);
    }
}