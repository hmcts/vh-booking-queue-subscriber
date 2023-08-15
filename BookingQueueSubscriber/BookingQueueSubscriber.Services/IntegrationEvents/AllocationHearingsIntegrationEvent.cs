namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class AllocationHearingsIntegrationEvent: IIntegrationEvent
    {
        public IList<HearingAllocationDto> Hearings { get; set; }

        public UserDto AllocatedCso { get; set; }
    }
}