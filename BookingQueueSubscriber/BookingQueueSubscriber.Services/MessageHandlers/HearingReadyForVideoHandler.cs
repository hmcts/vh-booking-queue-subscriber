using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoHandler : IMessageHandler<HearingIsReadyForVideoIntegrationEvent>
    {
        private readonly IConferenceCreationAndNotification _conferenceCreationAndNotification;

        public HearingReadyForVideoHandler(IConferenceCreationAndNotification conferenceCreationAndNotification)
        {
            _conferenceCreationAndNotification = conferenceCreationAndNotification;
        }

        public async Task HandleAsync(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            var request = new CreateConferenceAndNotifyRequest
            {
                Hearing = eventMessage.Hearing,
                ParticipantUsersToCreate = eventMessage.Participants,
                Participants = eventMessage.Participants,
                Endpoints = eventMessage.Endpoints
            };
            
            await _conferenceCreationAndNotification.CreateConferenceAndNotifyAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingIsReadyForVideoIntegrationEvent)integrationEvent);
        }
    }
}