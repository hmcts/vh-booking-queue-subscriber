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

        /// <summary>
        /// Part 1 of 3 of the new user journey
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        public Task SendNewUserWelcomeEmail(HearingDto hearing, ParticipantDto participant)
        {
            NotificationRequests.Add(AddNotificationRequestMapper.MapToNewUserWelcomeEmail(hearing, participant));
            return Task.FromResult(HttpStatusCode.OK);
        }

        /// <summary>
        /// Part 2 of 3 of the new user journey
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        public Task SendNewUserSingleDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant, string password)
        {
            NotificationRequests.Add(
                AddNotificationRequestMapper.MapToPostMay2023NewUserHearingConfirmationNotification(hearing,
                    participant, password));
            return Task.FromResult(HttpStatusCode.OK);
        }

        /// <summary>
        /// Part 2 of 3 of the new user journey. These users would have already received their welcome email
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        public Task SendExistingUserSingleDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant)
        {
            NotificationRequests.Add(
                AddNotificationRequestMapper.MapToPostMay2023ExistingUserHearingConfirmationNotification(hearing,
                    participant));
            return Task.FromResult(HttpStatusCode.OK);
        }

        /// <summary>
        /// Part 2 of 3 of the new user journey
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        public Task SendNewUserMultiDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant, string password,
            int days)
        {
            NotificationRequests.Add(
                AddNotificationRequestMapper.MapToPostMay2023NewUserMultiDayHearingConfirmationNotification(hearing,
                    participant, password, days));
            return Task.FromResult(HttpStatusCode.OK);
        }

        /// <summary>
        /// Part 2 of 3 of the new user journey. These users would have already received their welcome email
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        public Task SendExistingUserMultiDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant, int days)
        {
            NotificationRequests.Add(
                AddNotificationRequestMapper.MapToPostMay2023ExistingUserMultiHearingConfirmationNotification(hearing,
                    participant, days));
            return Task.FromResult(HttpStatusCode.OK);
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
