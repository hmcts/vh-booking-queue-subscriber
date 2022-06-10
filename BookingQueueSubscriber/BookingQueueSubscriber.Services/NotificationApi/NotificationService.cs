using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Contract.Responses;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    public interface INotificationService
    {
 
        Task SendNewUserEmailParticipantAsync(Guid hearingId, ParticipantDto participant, string password);

        Task SendHearingUpdateEmailAsync(HearingDetailsResponse originalHearing,
            HearingDetailsResponse updatedHearing, List<ParticipantResponse> participants = null);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationApiClient _notificationApiClient;

        public NotificationService(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        public async Task SendNewUserEmailParticipantAsync(Guid hearingId, ParticipantDto participant, string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                var request = AddNotificationRequestMapper.MapToNewUserNotification(hearingId, participant, password);
                await _notificationApiClient.CreateNewNotificationAsync(request);
            }
        }


        public async Task SendHearingUpdateEmailAsync(HearingDetailsResponse originalHearing,
            HearingDetailsResponse updatedHearing, List<ParticipantResponse> participants = null)
        {
            if (updatedHearing.IsGenericHearing())
            {
                return;
            }

            var @case = updatedHearing.Cases.First();
            var caseName = @case.Name;
            var caseNumber = @case.Number;

            var participantsToEmail = participants ?? updatedHearing.Participants;
            if (!updatedHearing.DoesJudgeEmailExist() || originalHearing.ConfirmedDate == null ||
                originalHearing.GroupId != originalHearing.Id)
            {
                participantsToEmail = participantsToEmail
                    .Where(x => !x.UserRoleName.Contains("Judge", StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }

            var requests = participantsToEmail
                .Select(participant =>
                    AddNotificationRequestMapper.MapToHearingAmendmentNotification(new HearingDto(), new ParticipantDto(),
                        originalHearing.ScheduledDateTime, updatedHearing.ScheduledDateTime))
                .ToList();

            await CreateNotifications(requests);
        }

        private async Task CreateNotifications(List<AddNotificationRequest> notificationRequests)
        {
            //if (_featureToggles.BookAndConfirmToggle())
                notificationRequests = notificationRequests.Where(req => !string.IsNullOrWhiteSpace(req.ContactEmail)).ToList();

            await Task.WhenAll(notificationRequests.Select(_notificationApiClient.CreateNewNotificationAsync));
        }


    }
}
