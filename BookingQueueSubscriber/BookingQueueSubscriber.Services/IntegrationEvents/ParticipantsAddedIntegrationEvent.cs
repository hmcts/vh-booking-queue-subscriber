namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ParticipantsAddedIntegrationEvent: IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public IList<ParticipantDto> Participants { get; set; }
    }
}