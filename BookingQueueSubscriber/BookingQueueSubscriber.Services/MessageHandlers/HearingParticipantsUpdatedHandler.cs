using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingParticipantsUpdatedHandler : IMessageHandler<HearingParticipantsUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly IUserCreationAndNotification _userCreationAndNotification;

        public HearingParticipantsUpdatedHandler(IVideoApiService videoApiService, IVideoWebService videoWebService,
            IUserCreationAndNotification userCreationAndNotification)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userCreationAndNotification = userCreationAndNotification;
        }

        public async Task HandleAsync(HearingParticipantsUpdatedIntegrationEvent eventMessage)
        {
            var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId, true);
            
            var newParticipantUsers = await _userCreationAndNotification.CreateUserAndNotifcationAsync(
                eventMessage.Hearing, eventMessage.NewParticipants);
            await _userCreationAndNotification.SendHearingNotificationAsync(eventMessage.Hearing, eventMessage.NewParticipants);

            var updateConferenceParticipantsRequest = new UpdateConferenceParticipantsRequest
            {
                ExistingParticipants =
                    eventMessage.ExistingParticipants.Select(x => ParticipantToUpdateParticipantMapper.MapToParticipantRequest(x)).ToList(),
                NewParticipants =
                    eventMessage.NewParticipants.Select(x => ParticipantToParticipantRequestMapper.MapToParticipantRequest(x)).ToList(),
                RemovedParticipants = eventMessage.RemovedParticipants,
                LinkedParticipants =
                    eventMessage.LinkedParticipants.Select(x => LinkedParticipantToRequestMapper.MapToLinkedParticipantRequest(x)).ToList(),
            };

            await _videoApiService.UpdateConferenceParticipantsAsync(conferenceResponse.Id, updateConferenceParticipantsRequest);
            await _videoWebService.PushParticipantsUpdatedMessage(conferenceResponse.Id, updateConferenceParticipantsRequest);

            await _userCreationAndNotification.HandleAssignUserToGroup(newParticipantUsers);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingParticipantsUpdatedIntegrationEvent)integrationEvent);
        }
    }
}