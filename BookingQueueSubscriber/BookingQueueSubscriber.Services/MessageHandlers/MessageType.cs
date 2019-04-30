namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public enum MessageType
    {
        HearingIsReadyForVideo,
        ParticipantAdded,
        ParticipantRemoved,
        HearingDetailsUpdated,
        HearingCancelled
    }
}