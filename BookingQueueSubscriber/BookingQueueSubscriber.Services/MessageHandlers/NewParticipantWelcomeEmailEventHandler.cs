using BookingQueueSubscriber.Services.NotificationApi;
using NotificationApi.Client;
using NotificationApi.Contract;
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
            var request = new AddNotificationRequest
            {
                HearingId = message.HearingId,
                MessageType = MessageType.Email,
                ContactEmail = message.ContactEmail,
                NotificationType = NotificationType.NewUserLipWelcome,
                PhoneNumber = message.ContactTelephone,
                Parameters = new Dictionary<string, string>
                {
                    {NotifyParams.Name, $"{message.FirstName} {message.LastName}" },
                    {NotifyParams.CaseName, message.CaseName },
                    {NotifyParams.CaseNumber, message.CaseNumber },
                }
            };

            await _notificationApiClient.CreateNewNotificationAsync(request);
        }
        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((NewParticipantWelcomeEmailEvent)integrationEvent);
        }
    }

}