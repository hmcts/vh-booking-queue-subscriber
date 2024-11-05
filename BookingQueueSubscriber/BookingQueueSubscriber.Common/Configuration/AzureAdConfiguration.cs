namespace BookingQueueSubscriber.Common.Configuration
{
    public class AzureAdConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public static string Authority => "https://login.microsoftonline.com/";
        public string TenantId { get; set; }
    }
}