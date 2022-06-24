using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoHandler : IMessageHandler<HearingIsReadyForVideoIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly IUserCreationAndNotification _userCreationAndNotification;


        public HearingReadyForVideoHandler(IVideoApiService videoApiService, IVideoWebService videoWebService,
             IUserCreationAndNotification userCreationAndNotification)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userCreationAndNotification = userCreationAndNotification;
        }

        public async Task HandleAsync(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            var newParticipantUsers = await _userCreationAndNotification.CreateUserAndNotifcationAsync(
                eventMessage.Hearing, eventMessage.Participants);
            await _userCreationAndNotification.SendHearingNotificationAsync(eventMessage.Hearing, eventMessage.Participants);

            var request = HearingToBookConferenceMapper.MapToBookNewConferenceRequest(eventMessage.Hearing,
                eventMessage.Participants, eventMessage.Endpoints);

            var conferenceDetailsResponse = await _videoApiService.BookNewConferenceAsync(request);
            
            await _videoWebService.PushNewConferenceAdded(conferenceDetailsResponse.Id);
            await _userCreationAndNotification.HandleAssignUserToGroup(newParticipantUsers);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingIsReadyForVideoIntegrationEvent)integrationEvent);
        }
    }
}