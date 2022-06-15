using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingParticipantsUpdatedHandler : IMessageHandler<HearingParticipantsUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public HearingParticipantsUpdatedHandler(IVideoApiService videoApiService, IVideoWebService videoWebService, IUserService userService,
            INotificationService notificationService)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userService = userService;
            _notificationService = notificationService;
        }

        public async Task HandleAsync(HearingParticipantsUpdatedIntegrationEvent eventMessage)
        {
            var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId, true);
            
            await new UserCreationAndNotificationHelper(_notificationService, _userService)
                .HandleUserCreationAndNotificationsAsync(eventMessage.Hearing, eventMessage.NewParticipants);

            var updateConferenceParticipantsRequest = new UpdateConferenceParticipantsRequest
            {
                ExistingParticipants =
                    eventMessage.ExistingParticipants.Select(x => ParticipantToUpdateParticipantMapper.MapToParticipantRequest(x)).ToList(),
                NewParticipants =
                    eventMessage.NewParticipants.Select(x => ParticipantToParticipantRequestMapper.MapToParticipantRequest(x)).ToList(),
                RemovedParticipants = eventMessage.RemovedParticipants,
                LinkedParticipants =
                    eventMessage.LinkedParticipants.Select(x => LinkedParticipantToRequestMapper.MapToLinkedParticipantRequest(x)).ToList(),
            };

            await _videoApiService.UpdateConferenceParticipantsAsync(conferenceResponse.Id, updateConferenceParticipantsRequest);
            await _videoWebService.PushParticipantsUpdatedMessage(conferenceResponse.Id, updateConferenceParticipantsRequest);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingParticipantsUpdatedIntegrationEvent)integrationEvent);
        }
    }
}