using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.UserApi
{
    [ExcludeFromCodeCoverage]
    public class UserServiceFake : IUserService
    {
        public List<User> Users { get; set; } = new List<User>();
        public  Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser)
        {
            var user = new User
            {
                UserId = $"{firstname}.{lastname}",
                UserName = $"{firstname}.{lastname}",
                ContactEmail = contactEmail
            };
            Users.Add(user);
            return Task.FromResult(user);

        }

        public Task AssignUserToGroup(string userId, string userRole)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task<UserProfile> GetUserByContactEmail(string emailAddress)
        {
            return Task.FromResult(new UserProfile
            {
                Email = emailAddress,
                UserName = emailAddress
            });
        }
    }
}
