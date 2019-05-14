namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public enum IntegrationEventType
    {
        HearingIsReadyForVideo,
        ParticipantAdded,
        ParticipantRemoved,
        HearingDetailsUpdated,
        HearingCancelled
    }
}