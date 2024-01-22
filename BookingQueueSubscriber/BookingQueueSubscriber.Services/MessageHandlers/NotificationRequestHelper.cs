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

        public static ExistingUserMultiDayHearingConfirmationRequest BuildExistingUserMultiDayHearingConfirmationRequest(
            HearingConfirmationForParticipantDto dto, int totalDays)
        {
            var cleanedCaseName = dto.CaseName.Replace($"Day 1 of {totalDays}", string.Empty).Trim();

            return new ExistingUserMultiDayHearingConfirmationRequest
            {
                Name = $"{dto.FirstName} {dto.LastName}",
                CaseName = cleanedCaseName,
                CaseNumber = dto.CaseNumber,
                ContactEmail = dto.ContactEmail,
                DisplayName = dto.DisplayName,
                HearingId = dto.HearingId,
                ParticipantId = dto.ParticipantId,
                Representee = dto.Representee,
                RoleName = dto.UserRole,
                ScheduledDateTime = dto.ScheduledDateTime,
                TotalDays = totalDays,
                Username = dto.Username
            };
        }
    }
}
