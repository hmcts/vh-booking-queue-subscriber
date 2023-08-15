namespace BookingQueueSubscriber.Services
{
    public static class HearingExtensions
    {
        public static bool IsGenericHearing(this HearingDto hearing)
        {
            return hearing.CaseType.Equals("Generic", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool HasScheduleAmended(this HearingDto hearing, HearingDto anotherHearing)
        {
            return hearing.ScheduledDateTime.Ticks != anotherHearing.ScheduledDateTime.Ticks;
        }


        public static bool HasEjdUsername(this ParticipantDto participant)
        {
            return !string.IsNullOrEmpty(participant.Username) && participant.Username.Contains("judiciary", StringComparison.CurrentCultureIgnoreCase);
        }
        public static bool HasVHAADUsername(this ParticipantDto participant)
        {
            return !string.IsNullOrEmpty(participant.Username) && participant.Username.Contains("hearings.reform.hmcts.net", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}