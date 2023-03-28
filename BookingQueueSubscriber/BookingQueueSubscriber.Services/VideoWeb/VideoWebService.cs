using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using BookingQueueSubscriber.Services.VideoWeb.Models;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public class VideoWebService : IVideoWebService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VideoWebService> _logger;
        public VideoWebService(HttpClient httpClient, ILogger<VideoWebService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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
            
            var json = MessageSerializer.Serialise(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(path, httpContent);

            _logger.LogDebug("PushParticipantsUpdatedMessage result: {Result}", result);
        }

        public async Task PushAllocationUpdatedMessage(AllocationUpdatedRequest request)
        {
            var path = $"internalevent/updatedAllocation";
            
            _logger.LogDebug("PushAllocationUpdatedMessage");
            var payload = MessageSerializer.Serialise(request);
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(path, httpContent);
            result.EnsureSuccessStatusCode();
            
            _logger.LogDebug("PushAllocationUpdatedMessage result: {Result}", result);
        }
    }
}
