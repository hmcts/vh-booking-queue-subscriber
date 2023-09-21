using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.NotificationApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class MultiDayHearingHandler : IMessageHandler<MultiDayHearingIntegrationEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUserCreationAndNotification _userCreationAndNotification;
        private readonly IFeatureToggles _featureToggles;

        public MultiDayHearingHandler(INotificationService notificationService, IFeatureToggles featureToggles,
            IUserCreationAndNotification userCreationAndNotification)
        {
            _notificationService = notificationService;
            _featureToggles = featureToggles;
            _userCreationAndNotification = userCreationAndNotification;
        }

        public async Task HandleAsync(MultiDayHearingIntegrationEvent eventMessage)
        {
            if (_featureToggles.UsePostMay2023Template())
            {
                await ProcessFeatureToggleOn(eventMessage);
            }
            else
            {
                await ProcessFeatureToggleOff(eventMessage);
            }
        }

        /// <summary>
        /// The new template journey combines the account details and hearing details into one email.
        /// As a result the handler will handle the creation of new accounts, including sending welcome emails, and the
        /// hearing confirmation email. 
        /// </summary>
        /// <param name="eventMessage"></param>
        private async Task ProcessFeatureToggleOn(MultiDayHearingIntegrationEvent eventMessage)
        {
            var newParticipantUsers = await _userCreationAndNotification.CreateUserAndSendNotificationAsync(
                eventMessage.Hearing, eventMessage.Participants);
            await _userCreationAndNotification.AssignUserToGroupForHearing(newParticipantUsers);
            var newUsernames  = newParticipantUsers.Select(x => x.Username).ToList();
            foreach (var participant in eventMessage.Participants)
            {
                var isUserNew = newUsernames.Contains(participant.Username);
                if (participant.IsIndividual() && isUserNew)
                {
                    var password = newParticipantUsers.First(x => x.Username == participant.Username).Password;
                    await _notificationService.SendNewUserMultiDayHearingConfirmationEmail(eventMessage.Hearing,
                        participant, password, eventMessage.TotalDays);
                }
                else if(participant.IsIndividual() && !isUserNew)
                {
                    await _notificationService.SendExistingUserMultiDayHearingConfirmationEmail(eventMessage.Hearing, participant, eventMessage.TotalDays);
                }
                else
                {
                    await _notificationService.SendNewMultiDayHearingConfirmationNotificationAsync(eventMessage.Hearing,
                        new List<ParticipantDto>() {participant}, eventMessage.TotalDays);
                }
            }
        }

        private async Task ProcessFeatureToggleOff(MultiDayHearingIntegrationEvent eventMessage)
        {
            await _notificationService.SendNewMultiDayHearingConfirmationNotificationAsync(eventMessage.Hearing,
                eventMessage.Participants, eventMessage.TotalDays);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((MultiDayHearingIntegrationEvent)integrationEvent);
        }
    }
}