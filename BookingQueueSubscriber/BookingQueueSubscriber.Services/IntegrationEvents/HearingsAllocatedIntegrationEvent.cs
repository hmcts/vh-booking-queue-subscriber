namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingsAllocatedIntegrationEvent: IIntegrationEvent
    {
        public IList<HearingDto> Hearings { get; set; }

        public JusticeUserDto AllocatedCso { get; set; }
    }
}