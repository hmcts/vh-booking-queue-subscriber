using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class EndpointUpdatedHandler : IMessageHandler<EndpointUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly ILogger<EndpointUpdatedHandler> _logger;
        private const int RetryLimit = 3;
        private const int RetrySleep = 3000;

        public EndpointUpdatedHandler(IVideoApiService videoApiService, IVideoWebService videoWebService, ILogger<EndpointUpdatedHandler> logger)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _logger = logger;
        }

        public async Task HandleAsync(EndpointUpdatedIntegrationEvent eventMessage)
        {
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            ParticipantDetailsResponse defenceAdvocate = null;
            
            if (!string.IsNullOrEmpty(eventMessage.DefenceAdvocate))
            {
                defenceAdvocate = await GetDefenceAdvocate(conference, eventMessage);
            }

            if (conference != null)
            {
                await _videoApiService.UpdateEndpointInConference(conference.Id, eventMessage.Sip, new UpdateEndpointRequest
                {
                    DisplayName = eventMessage.DisplayName,
                    DefenceAdvocate = defenceAdvocate?.Username
                });

                var endpoints = await _videoApiService.GetEndpointsForConference(conference.Id);

                var updateEndpointRequest = new UpdateConferenceEndpointsRequest
                {
                    ExistingEndpoints = endpoints.Where(x => x.SipAddress == eventMessage.Sip).ToList()
                };

                await _videoWebService.PushEndpointsUpdatedMessage(conference.Id, updateEndpointRequest);
            }
        }

        private async Task<ParticipantDetailsResponse> GetDefenceAdvocate(ConferenceDetailsResponse conference, EndpointUpdatedIntegrationEvent eventMessage)
        {
            ParticipantDetailsResponse defenceAdvocate = null;

            for (var retry = 0; retry <= RetryLimit; retry++)
            {
                if (conference == null)
                {
                    _logger.LogError("Unable to find conference by hearing id {HearingId}", eventMessage.HearingId);
                    break;
                }
                else
                {
                    defenceAdvocate = conference.Participants.SingleOrDefault(x => x.ContactEmail ==
                            eventMessage.DefenceAdvocate);

                    if (defenceAdvocate != null)
                    {
                        break;
                    }

                    if (retry == RetryLimit)
                    {
                        _logger.LogError("Unable to find defence advocate email by hearing id {HearingId}", eventMessage.HearingId);
                        break;
                    }
                }

                Thread.Sleep(RetrySleep);
                conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            }

            return defenceAdvocate;
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((EndpointUpdatedIntegrationEvent)integrationEvent);
        }
    }
}