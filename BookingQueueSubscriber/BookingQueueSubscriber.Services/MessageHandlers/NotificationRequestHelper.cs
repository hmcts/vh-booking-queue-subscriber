using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public static class NotificationRequestHelper
    {
        public static ExistingUserSingleDayHearingConfirmationRequest BuildExistingUserSingleDayHearingConfirmationRequest(
            HearingConfirmationForParticipantDto dto)
        {
            return new ExistingUserSingleDayHearingConfirmationRequest
            {
                HearingId = dto.HearingId,
                ContactEmail = dto.ContactEmail,
                ParticipantId = dto.ParticipantId,
                CaseName = dto.CaseName,
                CaseNumber = dto.CaseNumber,
                DisplayName = dto.DisplayName,
                Name = $"{dto.FirstName} {dto.LastName}",
                Representee = dto.Representee,
                Username = dto.Username,
                RoleName = dto.UserRole,
                ScheduledDateTime = dto.ScheduledDateTime
            };
        }
    }
}
