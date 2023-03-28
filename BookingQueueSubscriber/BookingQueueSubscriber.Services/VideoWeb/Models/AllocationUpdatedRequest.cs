using System.Collections.Generic;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.VideoWeb.Models;

public class AllocationUpdatedRequest
{
    public List<ConferenceDetailsResponse> Conferences { get; set; }
    public string AllocatedCsoUsername { get; set; }
}