using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;

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
        public async Task PushParticipantsAddedMessage(Guid conferenceId, AddParticipantsToConferenceRequest request)
        {
            logger.LogDebug("PushAddParticipantsMessage ConferenceId: {ConferenceId}, ParticipantCount: {ParticipantCount}", conferenceId, request.Participants.Count);

            // TODO make VideoWeb Client
            string json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var path = $"internalevent/ParticipantsAdded?conferenceId={conferenceId}";
            var test = httpClient.PostAsync($"internalevent/ParticipantsAdded?conferenceId={conferenceId}", httpContent).Result;
        }
    }
}
