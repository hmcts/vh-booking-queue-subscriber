using System.Diagnostics.CodeAnalysis;
using System.Net;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    [ExcludeFromCodeCoverage]
    public class NotificationServiceFake : INotificationService
    {
        public List<AddNotificationRequest> NotificationRequests { get; set; }

        public NotificationServiceFake()
        {
            NotificationRequests = new List<AddNotificationRequest>();
        }
        public Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string userPassword)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendNewSingleDayHearingConfirmationNotification(HearingDto hearing, IEnumerable<ParticipantDto> participants)
        {
            foreach (var participant in participants)
            {
                NotificationRequests.Add(AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant));
            }
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendNewUserWelcomeEmail(HearingDto hearing, ParticipantDto participant)
        {
            NotificationRequests.Add(AddNotificationRequestMapper.MapToNewUserWelcomeEmail(hearing, participant));
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendNewUserSingleDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant, string userPassword)
        {
            NotificationRequests.Add(AddNotificationRequestMapper.MapToNewUserNotification(hearing.HearingId, participant, userPassword));
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendExistingUserSingleDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant)
        {
            NotificationRequests.Add(AddNotificationRequestMapper.MapToNewUserAccountDetailsEmail(hearing, participant));
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}
