using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingsApi.Client;
using BookingsApi.Contract.Configuration;
using Microsoft.Extensions.Logging;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    public interface INotificationService
    {
        Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string password);
        Task SendNewHearingNotification(HearingDto hearing, IEnumerable<ParticipantDto> participants);
        Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime,
             IList<ParticipantDto> participants);

        Task SendMultiDayHearingNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationApiClient notificationApiClient, IBookingsApiClient bookingsApiClient, ILogger<NotificationService> logger)
        {
            _notificationApiClient = notificationApiClient;
            _bookingsApiClient = bookingsApiClient;
            _logger = logger;
        }

        public async Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                _logger.LogInformation("Creating Notification for new user {username}", participant.Username);
                var request = AddNotificationRequestMapper.MapToNewUserNotification(hearingId, participant, password);
                await _notificationApiClient.CreateNewNotificationAsync(request);
            }
        }
        public async Task SendNewHearingNotification(HearingDto hearing, IEnumerable<ParticipantDto> participants)
        {
            if (hearing.IsGenericHearing())
            {
                await ProcessGenericEmail(hearing, participants);
                return;
            }
            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
            var requests = participants
                .Select(participant => AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant, ejudFeatureFlag))
                .ToList();
            await CreateNotifications(requests);
            _logger.LogInformation("Created hearing notification for the users in the hearing {hearingid}", hearing.HearingId);
        }

        public async Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants)
        {
            if (hearing.IsGenericHearing())
            {
                return;
            }

            //var caseName = hearing.CaseName;
            //var caseNumber = hearing.CaseNumber;
            //if (!hearing.DoesJudgeEmailExist() || originalDateTime == null ||
            //    originalHearing.GroupId != originalHearing.Id)
            //{
            //    participantsToEmail = participantsToEmail
            //        .Where(x => !x.UserRole.Contains("Judge", StringComparison.CurrentCultureIgnoreCase))
            //        .ToList();
            //}
            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
            var requests = participants
                .Select(participant =>
                    AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant,
                        originalDateTime, hearing.ScheduledDateTime, ejudFeatureFlag))
                .ToList();

            await CreateNotifications(requests);
        }

        public async Task SendMultiDayHearingNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days)
        {
            if (hearing.IsGenericHearing())
            {
                await ProcessGenericEmail(hearing, participants); 
                return;
            }
            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
            var requests = participants
                .Select(participant => AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, days, ejudFeatureFlag))
                .ToList();

              await CreateNotifications(requests);
        }

        private async Task ProcessGenericEmail(HearingDto hearing, IEnumerable<ParticipantDto> participants)
        {
            if (string.Equals(hearing.HearingType, "Automated Test", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
            var notificationRequests = participants
                .Select(participant => AddNotificationRequestMapper.MapToDemoOrTestNotification(
                    hearing, participant, hearing.HearingType, ejudFeatureFlag))
                .Where(x => x != null)
                .ToList();

            if (!notificationRequests.Any()) return;
            await CreateNotifications(notificationRequests);
        }

        private async Task CreateNotifications(List<AddNotificationRequest> notificationRequests)
        {
            notificationRequests = notificationRequests.Where(req => !string.IsNullOrWhiteSpace(req.ContactEmail)).ToList();
            await Task.WhenAll(notificationRequests.Select(_notificationApiClient.CreateNewNotificationAsync));
        }
    }
}
