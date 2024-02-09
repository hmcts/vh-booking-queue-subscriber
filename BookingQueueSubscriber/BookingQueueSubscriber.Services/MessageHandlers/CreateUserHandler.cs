namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class CreateUserHandler : IMessageHandler<CreateUserIntegrationEvent>
    {
        private readonly IHearingService _hearingService;
        
        public CreateUserHandler(IHearingService hearingService)
        {
            _hearingService = hearingService;
        }
        
        public async Task HandleAsync(CreateUserIntegrationEvent eventMessage)
        {
            var message = eventMessage.Participant;
            await _hearingService.CreateUserForHearing(message.HearingId, 
                message.FirstName, message.LastName, message.ContactEmail, message.UserRole);
        }
        
        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((CreateUserIntegrationEvent)integrationEvent);
        }
    }
}
