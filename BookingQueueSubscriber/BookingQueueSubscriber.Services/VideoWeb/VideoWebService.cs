using System;
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
        private readonly HttpClient httpClient;
        private readonly ILogger<VideoWebService> logger;
        public VideoWebService(HttpClient _httpClient, ILogger<VideoWebService> _logger)
        {
            httpClient = _httpClient;
            logger = _logger;
        }
        public async Task PushParticipantsUpdatedMessage(Guid conferenceId, UpdateConferenceParticipantsRequest request)
        {
            var path = $"internalevent/ParticipantsUpdated?conferenceId={conferenceId}";
            
            logger.LogDebug("PushParticipantsUpdatedMessage ConferenceId: {ConferenceId}", conferenceId);

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
            var result = httpClient.PostAsync(path, httpContent).Result;

            logger.LogDebug("PushParticipantsUpdatedMessage result: {Result}", result);
        }



    }
}
