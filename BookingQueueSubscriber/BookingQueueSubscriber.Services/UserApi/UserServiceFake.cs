using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.UserApi
{
    public class UserServiceFake : IUserService
    {
        public  Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser)
        {
    
            return Task.FromResult(new User
            {
                UserId = "UserId",
                UserName = "UserName"
            });

        }

        public Task AssignUserToGroup(string userId, string userRole)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}
