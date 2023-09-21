using System.Diagnostics.CodeAnalysis;
using System.Net;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    [ExcludeFromCodeCoverage]
    public class NotificationServiceFake : INotificationService
    {
        public List<AddNotificationRequest> NotificationRequests { get; set; } = new();
        public bool EJudFeatureEnabled { get; set; }
        public Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string password)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
        public Task SendNewSingleDayHearingConfirmationNotification(HearingDto hearing, IEnumerable<ParticipantDto> participants)
        {
            foreach (var participant in participants)
            {
                NotificationRequests.Add(AddNotificationRequestMapper.MapToNewHearingConfirmationNotification(hearing, participant, EJudFeatureEnabled));
            }
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendNewUserWelcomeEmail(HearingDto hearing, ParticipantDto participant)
        {
            NotificationRequests = new List<AddNotificationRequest>
                {AddNotificationRequestMapper.MapToNewUserWelcomeEmail(hearing, participant)};
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendNewUserSingleDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant, string password)
        {
            throw new NotImplementedException();
        }

        public Task SendExistingUserSingleDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant)
        {
            throw new NotImplementedException();
        }

        public Task SendNewUserMultiDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant, string password,
            int days)
        {
            throw new NotImplementedException();
        }

        public Task SendExistingUserMultiDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant, int days)
        {
            throw new NotImplementedException();
        }

        public Task SendNewMultiDayHearingConfirmationNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
        
        public void ClearRequests()
        {
            NotificationRequests.Clear();
        }
    }
}
