using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class EndpointUpdatedHandler : IMessageHandler<EndpointUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly ILogger<EndpointUpdatedHandler> _logger;
        private const int RetryLimit = 3;
        private const int RetrySleep = 3000;

        public EndpointUpdatedHandler(IVideoApiService videoApiService, ILogger<EndpointUpdatedHandler> logger)
        {
            _videoApiService = videoApiService;
            _logger = logger;
        }

        public async Task HandleAsync(EndpointUpdatedIntegrationEvent eventMessage)
        {
            ParticipantDetailsResponse defenceAdvocate = null;
            ConferenceDetailsResponse conference = null;

            if (!string.IsNullOrEmpty(eventMessage.DefenceAdvocate))
            {
                for (var retry = 0; retry <= RetryLimit; retry++)
                {
                    conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);

                    if (conference == null)
                    {
                        _logger.LogError("Unable to find conference by hearing id {HearingId}", eventMessage.HearingId);
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
                }
            }

            await _videoApiService.UpdateEndpointInConference(conference.Id, eventMessage.Sip, new UpdateEndpointRequest
            {
                DisplayName = eventMessage.DisplayName,
                DefenceAdvocate = defenceAdvocate?.Username
            });
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((EndpointUpdatedIntegrationEvent)integrationEvent);
        }
    }
}