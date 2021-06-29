using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public class VideoWebService : IVideoWebService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<VideoWebService> logger;
        private const string Secret = "i98gvJLvhRS7pfSyM/fYPS2iyoe3lTP4AblwTLy1VoaN2BDVJXor0be0bezl3+pgMJWTnD/V7iOjdSOLXW56RA==";
        public VideoWebService(HttpClient _httpClient, ILogger<VideoWebService> _logger)
        {
            httpClient = _httpClient;
            logger = _logger;
        }
        public async Task PushParticipantsAddedMessage(Guid conferenceId, AddParticipantsToConferenceRequest request)
        {
            logger.LogDebug("PushAddParticipantsMessage ConferenceId: {ConferenceId}, ParticipantCount: {ParticipantCount}", conferenceId, request.Participants.Count);

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var path = $"internalevent/ParticipantsAdded?conferenceId={conferenceId}";
            var result = httpClient.PostAsync(path, httpContent).Result;

            logger.LogDebug("PushAddParticipantsMessage result: {Result}", result);
        }



    }
}
