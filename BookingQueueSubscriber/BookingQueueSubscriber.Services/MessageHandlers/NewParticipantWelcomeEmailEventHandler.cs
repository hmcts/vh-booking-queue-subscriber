using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class NewParticipantWelcomeEmailEventHandler : IMessageHandler<NewParticipantWelcomeEmailEvent>
    {
        private readonly INotificationApiClient _notificationApiClient;

        public NewParticipantWelcomeEmailEventHandler(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        public async Task HandleAsync(NewParticipantWelcomeEmailEvent eventMessage)
        {
            var message = eventMessage.WelcomeEmail;
            var request = new NewUserWelcomeEmailRequest { 
                Name = $"{message.FirstName} {message.LastName}",
                CaseName = message.CaseName, 
                CaseNumber = message.CaseNumber, 
                ContactEmail = message.ContactEmail, 
                HearingId = message.HearingId, 
                ParticipantId = message.ParticipnatId,  
                RoleName = message.UserRole };

            await _notificationApiClient.SendParticipantWelcomeEmailAsync(request);
        }
        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((NewParticipantWelcomeEmailEvent)integrationEvent);
        }
    }

}