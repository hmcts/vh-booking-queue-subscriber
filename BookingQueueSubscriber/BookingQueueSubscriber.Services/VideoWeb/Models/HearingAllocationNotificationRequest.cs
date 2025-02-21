namespace BookingQueueSubscriber.Services.VideoWeb.Models;

public class HearingAllocationNotificationRequest
{
    public string AllocatedCsoUserName { get; set; }
    public string AllocatedCsoFullName { get; set; }
    public Guid AllocatedCsoUserId { get; set; }
    public List<Guid> ConferenceIds { get; set; }
}