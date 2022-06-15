using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoHandler : IMessageHandler<HearingIsReadyForVideoIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public HearingReadyForVideoHandler(IVideoApiService videoApiService, IVideoWebService videoWebService, 
            IUserService userService, 
            INotificationService notificationService)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userService = userService;
            _notificationService = notificationService;
        }

        public async Task HandleAsync(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            await new UserCreationAndNotificationHelper(_notificationService, _userService)
                .HandleUserCreationAndNotificationsAsync(eventMessage.Hearing, eventMessage.Participants);

            var request = HearingToBookConferenceMapper.MapToBookNewConferenceRequest(eventMessage.Hearing,
                eventMessage.Participants, eventMessage.Endpoints);

            var conferenceDetailsResponse = await _videoApiService.BookNewConferenceAsync(request);
            
            await _videoWebService.PushNewConferenceAdded(conferenceDetailsResponse.Id);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingIsReadyForVideoIntegrationEvent)integrationEvent);
        }
    }
}