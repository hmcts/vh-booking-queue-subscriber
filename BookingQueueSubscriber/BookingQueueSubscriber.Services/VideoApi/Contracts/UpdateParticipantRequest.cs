namespace BookingQueueSubscriber.Services.VideoApi.Contracts
{
    public class UpdateParticipantRequest
    {
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Representee { get; set; }
        public string Fullname { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
    }
}