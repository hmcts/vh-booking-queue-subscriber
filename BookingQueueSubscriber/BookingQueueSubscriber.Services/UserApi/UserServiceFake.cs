﻿using System.Diagnostics.CodeAnalysis;
using System.Net;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.UserApi
{
    [ExcludeFromCodeCoverage]
    public class UserServiceFake : IUserService
    {
        public List<User> Users { get; set; } = new List<User>();
        public int UpdateUserAccountCount { get; private set; }
        
        public  Task<User> CreateNewUserForParticipantAsync(string firstname, string lastname, string contactEmail, bool isTestUser)
        {
            var user = new User
            {
                UserId = $"{firstname}.{lastname}",
                UserName = $"{firstname}.{lastname}",
                ContactEmail = contactEmail,
                Password = "password"
            };
            Users.Add(user);
            return Task.FromResult(user);

        }

        public Task AssignUserToGroup(string userId, string userRole)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
        
        public Task UpdateUserContactEmail(string existingContactEmail, string newContactEmail)
        {
            UpdateUserAccountCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}
