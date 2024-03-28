using System.Text.RegularExpressions;

namespace BookingQueueSubscriber.Services
{
    public static class HearingExtensions
    {
        private static readonly Regex EjudRegex = new ("@.*judiciary", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
        public static bool IsGenericHearing(this HearingDto hearing)
        {
            return hearing.CaseType.Equals("Generic", StringComparison.InvariantCultureIgnoreCase);
        }
        
        public static bool HasEjdUsername(this ParticipantDto participant)
        {
            return !string.IsNullOrEmpty(participant.Username) && EjudRegex.IsMatch(participant.Username);
        }
    }
}