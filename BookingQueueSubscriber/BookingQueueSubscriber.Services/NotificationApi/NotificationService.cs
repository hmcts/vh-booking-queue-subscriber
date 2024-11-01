using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    public interface INotificationService
    {
        /// <summary>
        /// Send the hearing amendment notification for a single hearing
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="originalDateTime"></param>
        /// <param name="participants"></param>
        /// <returns></returns>
        Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationApiClient _notificationApiClient;

        public NotificationService(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        public async Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants)
        {
            if (hearing.IsGenericHearing())
            {
                return;
            }

            var requests = participants
                .Select(participant =>
                    AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant, originalDateTime, hearing.ScheduledDateTime))
                .ToList();

            await CreateNotifications(requests);
        }

        private async Task CreateNotifications(List<AddNotificationRequest> notificationRequests)
        {
            notificationRequests = notificationRequests.Where(req => !string.IsNullOrWhiteSpace(req.ContactEmail)).ToList();
            await Task.WhenAll(notificationRequests.Select(_notificationApiClient.CreateNewNotificationAsync));
        }
    }
}
