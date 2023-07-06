using System.Collections.Generic;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.UserApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoRetryHandler : IMessageHandler<HearingIsReadyForVideoRetryIntegrationEvent>
    {
        private readonly IUserService _userService;
        private readonly IConferenceCreationAndNotification _conferenceCreationAndNotification;

        public HearingReadyForVideoRetryHandler(IUserService userService, IConferenceCreationAndNotification conferenceCreationAndNotification)
        {
            _userService = userService;
            _conferenceCreationAndNotification = conferenceCreationAndNotification;
        }

        public async Task HandleAsync(HearingIsReadyForVideoRetryIntegrationEvent eventMessage)
        {
            var participantUsersToCreate = new List<ParticipantDto>();
            
            foreach (var participant in eventMessage.Participants)
            {
                var user = await _userService.GetUserByContactEmail(participant.ContactEmail);
                if (user == null)
                {
                    participantUsersToCreate.Add(participant);
                }
            }

            var request = new CreateConferenceAndNotifyRequest
            {
                Hearing = eventMessage.Hearing,
                ParticipantUsersToCreate = participantUsersToCreate,
                Participants = eventMessage.Participants,
                Endpoints = eventMessage.Endpoints
            };
            
            await _conferenceCreationAndNotification.CreateConferenceAndNotifyAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingIsReadyForVideoRetryIntegrationEvent)integrationEvent);
        }
    }
}
