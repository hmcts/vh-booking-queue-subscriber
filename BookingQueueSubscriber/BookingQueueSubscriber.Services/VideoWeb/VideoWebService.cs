using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public class VideoWebService : IVideoWebService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VideoWebService> _logger;
        public VideoWebService(HttpClient httpClient, ILogger<VideoWebService> logger)
        {
            this._httpClient = httpClient;
            this._logger = logger;
        }

        public async Task PushNewConferenceAdded(Guid conferenceId)
        {
            var path = $"internalevent/ConferenceAdded?conferenceId={conferenceId}";

            _logger.LogDebug("PushNewConferenceAdded ConferenceId: {ConferenceId}", conferenceId);

            await _httpClient.PostAsync(path, null);
        }

        public async Task PushParticipantsUpdatedMessage(Guid conferenceId, UpdateConferenceParticipantsRequest request)
        {
            var path = $"internalevent/ParticipantsUpdated?conferenceId={conferenceId}";
            
            _logger.LogDebug("PushParticipantsUpdatedMessage ConferenceId: {ConferenceId}", conferenceId);

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
            var result = await _httpClient.PostAsync(path, httpContent);

            _logger.LogDebug("PushParticipantsUpdatedMessage result: {Result}", result);
        }
        
        public async Task PushAllocationToCsoUpdatedMessage(AllocationHearingsToCsoRequest request)
        {
            var path = $"internalevent/AllocationHearings";
            
            _logger.LogDebug("PushAllocationToCsoUpdatedMessage");

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
            var result = await _httpClient.PostAsync(path, httpContent);

            _logger.LogDebug("PushAllocationToCsoUpdatedMessage result: {Result}", result);
        }
    }
}
