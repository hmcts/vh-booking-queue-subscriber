using System.Globalization;
using TimeZoneConverter;

namespace BookingQueueSubscriber.Services
{ 
    public static class DateExtensions
    {
        private static readonly TimeZoneInfo BritishZone = TZConvert.GetTimeZoneInfo("Europe/London");
        private static readonly CultureInfo CultureInfo = new CultureInfo("en-GB");
        public static string ToEmailDateGbLocale(this DateTime datetime)
        {
            var gmtDate = TimeZoneInfo.ConvertTimeFromUtc(datetime, BritishZone);
            return gmtDate.ToString("d MMMM yyyy", CultureInfo);
        }
        
        public static string ToEmailTimeGbLocale(this DateTime datetime)
        {
            var gmtDate = TimeZoneInfo.ConvertTimeFromUtc(datetime, BritishZone);
            return gmtDate.ToString("h:mm tt", CultureInfo)
                .ToUpper();
        }
    }
}