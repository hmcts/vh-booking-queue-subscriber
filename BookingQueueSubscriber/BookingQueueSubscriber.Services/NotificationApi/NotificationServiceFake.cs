using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.NotificationApi
{
 
    public class NotificationServiceFake : INotificationService
    {

        public Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string password)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
        public Task SendNewHearingNotification(HearingDto hearing, IList<ParticipantDto> participants)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendMultiDayHearingNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}
