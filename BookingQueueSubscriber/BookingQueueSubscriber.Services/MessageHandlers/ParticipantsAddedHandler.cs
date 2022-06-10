using System;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingsApi.Client;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantsAddedHandler : IMessageHandler<ParticipantsAddedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public ParticipantsAddedHandler(IVideoApiService videoApiService, IVideoWebService videoWebService, IUserService userService, 
            INotificationService notificationService)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userService = userService;
            _notificationService = notificationService;
        }

        public async Task HandleAsync(ParticipantsAddedIntegrationEvent eventMessage)
        {
            foreach (var participant in eventMessage.Participants)
            {
                await CreateUserAndSendNotificationAsync(eventMessage.HearingId, participant);
            }

            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            var request = new AddParticipantsToConferenceRequest
            {
                Participants = eventMessage.Participants
                    .Select(ParticipantToParticipantRequestMapper.MapToParticipantRequest).ToList()
            };

            await _videoApiService.AddParticipantsToConference(conference.Id, request);

            var updateConferenceParticipantsRequest = new UpdateConferenceParticipantsRequest
            {
                NewParticipants =
                    eventMessage.Participants.Select(x => ParticipantToParticipantRequestMapper.MapToParticipantRequest(x)).ToList(),
            };
            await _videoWebService.PushParticipantsUpdatedMessage(conference.Id, updateConferenceParticipantsRequest);
        }

        private async Task CreateUserAndSendNotificationAsync(Guid hearingId, ParticipantDto participant)
        {
            User user = null;
            if (string.IsNullOrWhiteSpace(participant.Username) && !string.Equals(participant.UserRole, RoleNames.Judge))
            {
                user = await _userService.CreateNewUserForParticipantAsync(participant.FirstName,
                    participant.LastName, participant.ContactEmail, false);
                participant.Username = user.UserName;
                // Update participant with the user name though bookings api.
            }

            if (user != null)
            {
                await _notificationService.SendNewUserEmailParticipantAsync(hearingId, participant, user.Password);
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantsAddedIntegrationEvent)integrationEvent);
        }
    }
}