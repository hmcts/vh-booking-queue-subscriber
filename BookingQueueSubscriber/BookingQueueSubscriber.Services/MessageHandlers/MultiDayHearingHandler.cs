using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.NotificationApi;
using LaunchDarkly.Sdk;
using UserApi.Client;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class MultiDayHearingHandler : IMessageHandler<MultiDayHearingIntegrationEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUserApiClient _userApiClient;
        private readonly IFeatureToggles _featureToggles;

        public MultiDayHearingHandler(INotificationService notificationService, IFeatureToggles featureToggles)
        {
            _notificationService = notificationService;
            _featureToggles = featureToggles;
        }

        public async Task HandleAsync(MultiDayHearingIntegrationEvent eventMessage)
        {
            foreach (var participant in eventMessage.Participants)
            {
                bool isNewUser = true; // TODO: determine if the user is new somehow. Maybe use redis?
                if (participant.IsIndividual() && _featureToggles.UsePostMay2023Template() && !isNewUser)
                {
                   await _notificationService.SendExistingUserMultiDayHearingConfirmationEmail(eventMessage.Hearing, participant, eventMessage.TotalDays);
                }
                else if(participant.IsIndividual() && _featureToggles.UsePostMay2023Template() && isNewUser)
                {
                    // var password = await _userApiClient.ResetUserPasswordAsync(participant.Username);
                    var password = "password"; // TODO: generate a password. call user api to reset it?
                    await _notificationService.SendNewUserMultiDayHearingConfirmationEmail(eventMessage.Hearing, participant, password, eventMessage.TotalDays);
                }
                else
                {
                    await _notificationService.SendNewMultiDayHearingConfirmationNotificationAsync(eventMessage.Hearing,
                        new List<ParticipantDto>() {participant}, eventMessage.TotalDays);
                }
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((MultiDayHearingIntegrationEvent)integrationEvent);
        }
    }
}