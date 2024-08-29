using System.Text;
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
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task PushNewConferenceAdded(Guid conferenceId)
        {
            var path = $"internalevent/ConferenceAdded?conferenceId={conferenceId}";

            _logger.LogDebug("PushNewConferenceAdded ConferenceId: {ConferenceId}", conferenceId);

            var result = await _httpClient.PostAsync(path, null);
            result.EnsureSuccessStatusCode();
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
            
            _logger.LogDebug("PushParticipantsUpdatedMessage json: {Json} to {Url}", json, $"{_httpClient.BaseAddress}/{path}");

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(path, httpContent);
            result.EnsureSuccessStatusCode();
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

        public async Task PushUnlinkedParticipantFromEndpoint(Guid conferenceId, string participantUserName, string jvsEndpointName)
        {
            var path = $"internalevent/UnlinkedParticipantFromEndpoint?conferenceId={conferenceId}&participant={participantUserName}&endpoint={jvsEndpointName}";
            _logger.LogDebug("PushNewConferenceAdded ConferenceId: {ConferenceId}", conferenceId);
            await _httpClient.PostAsync(path, null);
        }

        public async Task PushLinkedNewParticipantToEndpoint(Guid conferenceId, string participantUserName, string jvsEndpointName)
        {
            var path = $"internalevent/LinkedNewParticipantToEndpoint?conferenceId={conferenceId}&participant={participantUserName}&endpoint={jvsEndpointName}";
            _logger.LogDebug("PushNewConferenceAdded ConferenceId: {ConferenceId}", conferenceId);
            await _httpClient.PostAsync(path, null);
        }

        public async Task PushCloseConsultationBetweenEndpointAndParticipant(Guid conferenceId, string participantUserName, string jvsEndpointName)
        {
            var path = $"internalevent/CloseConsultationBetweenEndpointAndParticipant?conferenceId={conferenceId}&participant={participantUserName}&endpoint={jvsEndpointName}";
            _logger.LogDebug("PushNewConferenceAdded ConferenceId: {ConferenceId}", conferenceId);
            await _httpClient.PostAsync(path, null);
        }

        public async Task PushEndpointsUpdatedMessage(Guid conferenceId, UpdateConferenceEndpointsRequest request)
        {
            var path = $"internalevent/EndpointsUpdated?conferenceId={conferenceId}";

            _logger.LogDebug("PushEndpointsUpdatedMessage ConferenceId: {ConferenceId}", conferenceId);

            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(path, httpContent);

            result.EnsureSuccessStatusCode();

            _logger.LogDebug("PushEndpointsUpdatedMessage result: {Result}", result);
        }
        
        public async Task PushHearingCancelledMessage(Guid hearingId)
        {
            var path = $"internalevent/HearingCancelled?hearingId={hearingId}";

            _logger.LogDebug("PushHearingCancelled HearingId: {HearingId}", hearingId);

            var result = await _httpClient.PostAsync(path, null);
            result.EnsureSuccessStatusCode();
        }
        
        public async Task PushHearingDateTimeChangedMessage(Guid hearingId)
        {
            var path = $"internalevent/HearingDateTimeChanged?hearingId={hearingId}";

            _logger.LogDebug("PushHearingDateTimeChanged HearingId: {HearingId}", hearingId);

            var result = await _httpClient.PostAsync(path, null);
            result.EnsureSuccessStatusCode();
        }
    }
}
