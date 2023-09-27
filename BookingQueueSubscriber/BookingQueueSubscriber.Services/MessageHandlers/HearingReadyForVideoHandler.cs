using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingsApi.Client;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoHandler : IMessageHandler<HearingIsReadyForVideoIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly IUserCreationAndNotification _userCreationAndNotification;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly IFeatureToggles _featureToggles;
        private readonly INotificationService _notificationService;

        public HearingReadyForVideoHandler(IVideoApiService videoApiService, IVideoWebService videoWebService,
            IUserCreationAndNotification userCreationAndNotification, IBookingsApiClient bookingsApiClient, IFeatureToggles featureToggles, INotificationService notificationService)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userCreationAndNotification = userCreationAndNotification;
            _bookingsApiClient = bookingsApiClient;
            _featureToggles = featureToggles;
            _notificationService = notificationService;
        }

        public async Task HandleAsync(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            if (!eventMessage.Hearing.IsMultiDayHearing())
            {
                // create user and notification only if it is not a multiday hearing
                var newParticipantUsers = await _userCreationAndNotification.CreateUserAndNotifcationAsync(
                    eventMessage.Hearing, eventMessage.Participants);
                
                if (_featureToggles.UsePostMay2023Template())
                {
                    // The new template journey combines the account details and hearing details into one email.
                    // we need to remove the new user created in the previous step because they already received notifications for hearing
                    await _userCreationAndNotification.SendHearingNotificationAsync(eventMessage.Hearing,
                        eventMessage.Participants.Where(x => newParticipantUsers.All(y=>y.Username != x.Username)));
                }
                else
                {
                    // The old template journey sends the welcome email and the hearing confirmation email separately.
                    await _userCreationAndNotification.SendHearingNotificationAsync(eventMessage.Hearing,
                        eventMessage.Participants.Where(x => x.SendHearingNotificationIfNew));
                }
                
                
                await _userCreationAndNotification.HandleAssignUserToGroup(newParticipantUsers);
                
            }

            await CreateNewConferenceAndPublishInternalEvent(eventMessage);
        }
        
        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingIsReadyForVideoIntegrationEvent) integrationEvent);
        }

        private async Task CreateNewConferenceAndPublishInternalEvent(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            var request = HearingToBookConferenceMapper.MapToBookNewConferenceRequest(eventMessage.Hearing,
                eventMessage.Participants, eventMessage.Endpoints);
            var conferenceDetailsResponse = await _videoApiService.BookNewConferenceAsync(request);
            await _bookingsApiClient.UpdateBookingStatusAsync(eventMessage.Hearing.HearingId, new UpdateBookingStatusRequest
                {Status = UpdateBookingStatus.Created, UpdatedBy = "System"});
            await _videoWebService.PushNewConferenceAdded(conferenceDetailsResponse.Id);
        }
    }
}