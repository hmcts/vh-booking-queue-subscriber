using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class NewParticipantHearingConfirmationHandler : IMessageHandler<NewParticipantHearingConfirmationEvent>
    {
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IHearingService _hearingService;

        public NewParticipantHearingConfirmationHandler(INotificationApiClient notificationApiClient,
            IHearingService hearingService)
        {
            _notificationApiClient = notificationApiClient;
            _hearingService = hearingService;
        }

        public async Task HandleAsync(NewParticipantHearingConfirmationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;

            var newUser = await _hearingService.CreateUserForHearing(message.HearingId,
                message.FirstName, message.LastName, message.ContactEmail, message.UserRole);

            var request = new NewUserSingleDayHearingConfirmationRequest
            {
                HearingId = message.HearingId,
                ContactEmail = message.ContactEmail,
                ParticipantId = message.ParticipantId,
                CaseName = message.CaseName,
                ScheduledDateTime = message.ScheduledDateTime,
                Username = newUser.UserName,
                RoleName = message.UserRole,
                CaseNumber = message.CaseNumber,
                RandomPassword = newUser.Password,
                Name = $"{message.FirstName} {message.LastName}"
            };
            
            await _notificationApiClient.SendParticipantSingleDayHearingConfirmationForNewUserEmailAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((NewParticipantHearingConfirmationEvent)integrationEvent);
        }
    }
}