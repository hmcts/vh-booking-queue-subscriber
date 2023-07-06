using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingsApi.Client;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;

namespace BookingQueueSubscriber.Services
{
    public interface IConferenceCreationAndNotification
    {
        Task CreateConferenceAndNotifyAsync(CreateConferenceAndNotifyRequest request);
    }

    public class CreateConferenceAndNotifyRequest
    {
        public HearingDto Hearing { get; set; }
        public IList<ParticipantDto> ParticipantUsersToCreate { get; set; }
        public IList<ParticipantDto> Participants { get; set; }
        public IList<EndpointDto> Endpoints { get; set; }
    }
    
    public class ConferenceCreationAndNotification : IConferenceCreationAndNotification
    {
        private readonly IUserCreationAndNotification _userCreationAndNotification;
        private readonly IVideoApiService _videoApiService;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly IVideoWebService _videoWebService;

        public ConferenceCreationAndNotification(IUserCreationAndNotification userCreationAndNotification,
            IVideoApiService videoApiService, IBookingsApiClient bookingsApiClient, IVideoWebService videoWebService)
        {
            _userCreationAndNotification = userCreationAndNotification;
            _videoApiService = videoApiService;
            _bookingsApiClient = bookingsApiClient;
            _videoWebService = videoWebService;
        }
        
        public async Task CreateConferenceAndNotifyAsync(CreateConferenceAndNotifyRequest request)
        {
            var newParticipantUsers = await _userCreationAndNotification.CreateUserAndNotifcationAsync(
                request.Hearing, request.ParticipantUsersToCreate);

            await _userCreationAndNotification.HandleAssignUserToGroup(newParticipantUsers);
            
            if (!request.Hearing.GroupId.HasValue || request.Hearing.GroupId.GetValueOrDefault() == Guid.Empty)
            {
                // Not a multiday hearing
                await _userCreationAndNotification.SendHearingNotificationAsync(request.Hearing,
                    request.ParticipantUsersToCreate.Where(x => x.SendHearingNotificationIfNew));
            }

            var bookNewConferenceRequest = HearingToBookConferenceMapper.MapToBookNewConferenceRequest(request.Hearing,
                request.Participants, request.Endpoints);

            var conferenceDetailsResponse = await _videoApiService.BookNewConferenceAsync(bookNewConferenceRequest);
            await _bookingsApiClient.UpdateBookingStatusAsync(request.Hearing.HearingId, new UpdateBookingStatusRequest
                { Status = UpdateBookingStatus.Created, UpdatedBy = "System" });
            
            await _videoWebService.PushNewConferenceAdded(conferenceDetailsResponse.Id);
        }
    }
}
