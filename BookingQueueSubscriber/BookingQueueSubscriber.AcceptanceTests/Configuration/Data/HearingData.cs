namespace BookingQueueSubscriber.AcceptanceTests.Configuration.Data
{
    public static class HearingData
    {
        public const bool AUDIO_RECORDING_REQUIRED = false;
        public const string AUTOMATED_CASE_NAME_PREFIX = "Automated Test";
        public const string CANCELLATION_REASON = "Cancellation reason";
        public const string CASE_TYPE_NAME = "Civil Money Claims";
        public static string CREATED_BY(string usernameStem) => $"automation_test@{usernameStem}";
        public const string HEARING_ROOM_NAME = "Room 1";
        public const string HEARING_TYPE_NAME = "Application to Set Judgment Aside";
        public const bool IS_LEAD_CASE = false;
        public const string OTHER_INFORMATION = "Other information";
        public const bool QUESTIONNAIRE_NOT_REQUIRED = true;
        public const int SCHEDULED_DURATION = 60;
        public const string UPDATED_TEXT = "UPDATED";
        public const string VENUE_NAME = "Birmingham Civil and Family Justice Centre";
        public const string VENUE_NAME_ALTERNATIVE = "Manchester Civil and Family Justice Centre";
        public const string VIDEO_EVENT_REASON = "Automated test";
    }
}
