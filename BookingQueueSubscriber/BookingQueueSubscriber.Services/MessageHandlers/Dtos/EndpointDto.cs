namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class EndpointDto
    {
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public string DefenceAdvocateContactEmail { get; set; }
        public ConferenceRole Role { get; set; }
    }
}