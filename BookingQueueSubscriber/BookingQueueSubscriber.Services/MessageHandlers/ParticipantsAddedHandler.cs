using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantsAddedHandler : IMessageHandler<ParticipantsAddedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly IUserCreationAndNotification _userCreationAndNotification;
        private readonly ILogger<ParticipantsAddedHandler> _logger;

        public ParticipantsAddedHandler(IVideoApiService videoApiService, IVideoWebService videoWebService, 
            IUserCreationAndNotification userCreationAndNotification, ILogger<ParticipantsAddedHandler> logger)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userCreationAndNotification = userCreationAndNotification;
            _logger = logger;
        }

        public async Task HandleAsync(ParticipantsAddedIntegrationEvent eventMessage)
        {
            var newParticipantUsers = await _userCreationAndNotification.CreateUserAndNotifcationAsync(
                eventMessage.Hearing, eventMessage.Participants);
            await _userCreationAndNotification.SendHearingNotificationAsync(eventMessage.Hearing, eventMessage.Participants);

            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId);
            _logger.LogInformation("Update participant list for Conference {ConferenceId}", conference.Id);
            var request = new AddParticipantsToConferenceRequest
            {
                Participants = eventMessage.Participants
                    .Select(ParticipantToParticipantRequestMapper.MapToParticipantRequest).ToList()
            };

            await _videoApiService.AddParticipantsToConference(conference.Id, request);

            var updateConferenceParticipantsRequest = new UpdateConferenceParticipantsRequest
            {
                NewParticipants =
                    eventMessage.Participants.Select(ParticipantToParticipantRequestMapper.MapToParticipantRequest).ToList(),
            };
            await _videoWebService.PushParticipantsUpdatedMessage(conference.Id, updateConferenceParticipantsRequest);
            await _userCreationAndNotification.HandleAssignUserToGroup(newParticipantUsers);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantsAddedIntegrationEvent)integrationEvent);
        }
    }


}