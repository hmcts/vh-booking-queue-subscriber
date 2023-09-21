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
        private readonly INotificationService _notificationService;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly IFeatureToggles _featureToggles;

        public HearingReadyForVideoHandler(IVideoApiService videoApiService, IVideoWebService videoWebService,
            IUserCreationAndNotification userCreationAndNotification, IBookingsApiClient bookingsApiClient,
            INotificationService notificationService, IFeatureToggles featureToggles)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userCreationAndNotification = userCreationAndNotification;
            _bookingsApiClient = bookingsApiClient;
            _notificationService = notificationService;
            _featureToggles = featureToggles;
        }

        public async Task HandleAsync(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            // Send the hearing confirmation email
            if (_featureToggles.UsePostMay2023Template())
            {
                await ProcessNotificationWithNewTemplateToggleOn(eventMessage);
            }
            else
            {
                await ProcessNotificationWithNewTemplateToggleOff(eventMessage);
            }

            await CreateNewConferenceAndPublishInternalEvent(eventMessage);
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

        private async Task ProcessNotificationWithNewTemplateToggleOff(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            // Create new users. if new template is toggled on then the welcome and new confirmation email is sent
            var newParticipantUsers = await _userCreationAndNotification.CreateUserAndSendNotificationAsync(
                eventMessage.Hearing, eventMessage.Participants);
            await _userCreationAndNotification.AssignUserToGroupForHearing(newParticipantUsers);
            if (!eventMessage.Hearing.IsMultiDayHearing())
            {
                await SendSingleDayHearingConfirmationEmail(eventMessage, newParticipantUsers);
            }
        }

        private async Task ProcessNotificationWithNewTemplateToggleOn(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            if (!eventMessage.Hearing.IsMultiDayHearing())
            {
                // Create new users. if new template is toggled on then the welcome and new confirmation email is sent
                var newParticipantUsers = await _userCreationAndNotification.CreateUserAndSendNotificationAsync(
                    eventMessage.Hearing, eventMessage.Participants);
                await _userCreationAndNotification.AssignUserToGroupForHearing(newParticipantUsers);
                await SendSingleDayHearingConfirmationEmail(eventMessage, newParticipantUsers);
            }
        }

        private async Task SendSingleDayHearingConfirmationEmail(HearingIsReadyForVideoIntegrationEvent eventMessage,
            IList<UserDto> newParticipantUsers)
        {
            var newUsernames  = newParticipantUsers.Select(x => x.Username).ToList();
            foreach (var participant in eventMessage.Participants)
            {
                var isUserNew = newUsernames.Contains(participant.Username);

                if (participant.IsIndividual() && _featureToggles.UsePostMay2023Template() && isUserNew)
                {
                    // the new creation process sends the welcome email and the new confirmation email that includes account details
                    continue;
                }
                
                if (participant.IsIndividual() && _featureToggles.UsePostMay2023Template() && !isUserNew)
                {
                    await _notificationService.SendExistingUserSingleDayHearingConfirmationEmail(eventMessage.Hearing,
                        participant);
                }
                else
                {
                    await _notificationService.SendNewSingleDayHearingConfirmationNotification(eventMessage.Hearing,
                        new List<ParticipantDto>() {participant});
                }
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingIsReadyForVideoIntegrationEvent)integrationEvent);
        }
    }
}