using System;
using System.Collections.Generic;
using System.Linq;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Contract.Responses;
using NotificationApi.Contract;
using NotificationApi.Contract.Requests;
using UserNotificationQueueSubscriber.Services;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    public static class AddNotificationRequestMapper
    {
        public static AddNotificationRequest MapToNewUserNotification(Guid hearingId, ParticipantDto participant, string password)
        {
            var parameters = new Dictionary<string, string>
            {
                {NotifyParams.Name, $"{participant.FirstName} {participant.LastName}"},
                {NotifyParams.UserName, $"{participant.Username}"},
                {NotifyParams.RandomPassword, $"{password}"}
            };

            var notificationType =
                participant.UserRole.Contains(RoleNames.Individual, StringComparison.InvariantCultureIgnoreCase)
                    ? NotificationType.CreateIndividual
                    : NotificationType.CreateRepresentative;
        
            var addNotificationRequest = new AddNotificationRequest
            {
                HearingId = hearingId,
                MessageType = MessageType.Email,
                ContactEmail = participant.ContactEmail,
                NotificationType = notificationType,
                ParticipantId = participant.ParticipantId,
                PhoneNumber = participant.ContactTelephone,
                Parameters = parameters
            };
            return addNotificationRequest;
        }

        public static AddNotificationRequest MapToHearingAmendmentNotification(HearingDto hearing, ParticipantDto participant, DateTime originalDateTime,
            DateTime newDateTime)
        {
            var parameters = new Dictionary<string, string>
            {
                {NotifyParams.CaseName, hearing.CaseName},
                {NotifyParams.CaseNumber,hearing.CaseNumber},
                {NotifyParams.OldTime, originalDateTime.ToEmailTimeGbLocale()},
                {NotifyParams.NewTime, newDateTime.ToEmailTimeGbLocale()},
                {NotifyParams.OldDayMonthYear, originalDateTime.ToEmailDateGbLocale()},
                {NotifyParams.NewDayMonthYear, newDateTime.ToEmailDateGbLocale()}
            };

            NotificationType notificationType;
            if (participant.UserRole.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.HearingAmendmentEJudJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (participant.UserRole.Contains(RoleNames.JudicialOfficeHolder, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.HearingAmendmentEJudJoh;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");

            }
            else if (participant.UserRole.Contains(RoleNames.Representative, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.HearingAmendmentRepresentative;
                parameters.Add(NotifyParams.ClientName, participant.Representee);
                parameters.Add(NotifyParams.SolicitorName, $"{participant.FirstName} {participant.LastName}");
            }
            else
            {
                notificationType = NotificationType.HearingAmendmentLip;
                parameters.Add(NotifyParams.Name, $"{participant.FirstName} {participant.LastName}");
            }

            return new AddNotificationRequest
            {
                HearingId = hearing.HearingId,
                MessageType = MessageType.Email,
                ContactEmail = participant.ContactEmail,
                NotificationType = notificationType,
                ParticipantId = participant.ParticipantId,
                PhoneNumber = participant.ContactTelephone,
                Parameters = parameters
            };
        }

        public static AddNotificationRequest MapToHearingConfirmationNotification(HearingDetailsResponse hearing,
            ParticipantResponse participant)
        {
            var parameters = InitConfirmReminderParams(hearing);

            NotificationType notificationType;
            if (participant.UserRoleName.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase) &&
                hearing.IsJudgeEmailEJud())
            {
                notificationType = NotificationType.HearingConfirmationEJudJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (participant.UserRoleName.Contains(RoleNames.StaffMember, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.HearingConfirmationStaffMember;
                parameters.Add(NotifyParams.StaffMember, $"{participant.FirstName} {participant.LastName}");
                parameters.Add(NotifyParams.UserName, participant.Username);
            }
            else if (participant.UserRoleName.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase) &&
                     !hearing.IsJudgeEmailEJud())
            {
                notificationType = NotificationType.HearingConfirmationJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
                parameters.Add(NotifyParams.CourtroomAccountUserName, participant.Username);
                participant.ContactEmail = hearing.GetJudgeEmail();
                participant.TelephoneNumber = hearing.GetJudgePhone();
            }
            else if (participant.UserRoleName.Contains(RoleNames.JudicialOfficeHolder,
                StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = hearing.IsParticipantAEJudJudicialOfficeHolder(participant.Id) ? NotificationType.HearingConfirmationEJudJoh : NotificationType.HearingConfirmationJoh;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");
            }
            else if (participant.UserRoleName.Contains(RoleNames.Representative, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.HearingConfirmationRepresentative;
                parameters.Add(NotifyParams.ClientName, participant.Representee);
                parameters.Add(NotifyParams.SolicitorName, $"{participant.FirstName} {participant.LastName}");
            }
            else
            {
                notificationType = NotificationType.HearingConfirmationLip;
                parameters.Add(NotifyParams.Name, $"{participant.FirstName} {participant.LastName}");
            }

            return new AddNotificationRequest
            {
                HearingId = hearing.Id,
                MessageType = MessageType.Email,
                ContactEmail = participant.ContactEmail,
                NotificationType = notificationType,
                ParticipantId = participant.Id,
                PhoneNumber = participant.TelephoneNumber,
                Parameters = parameters
            };
        }

        public static AddNotificationRequest MapToMultiDayHearingConfirmationNotification(
            HearingDetailsResponse hearing,
            ParticipantResponse participant, int days)
        {
            var @case = hearing.Cases.First();
            var cleanedCaseName = @case.Name.Replace($"Day 1 of {days}", string.Empty).Trim();
            var parameters = new Dictionary<string, string>
            {
                {NotifyParams.CaseName, cleanedCaseName},
                {NotifyParams.CaseNumber, @case.Number},
                {NotifyParams.Time, hearing.ScheduledDateTime.ToEmailTimeGbLocale()},
                {NotifyParams.StartDayMonthYear, hearing.ScheduledDateTime.ToEmailDateGbLocale()},
                {NotifyParams.NumberOfDays, days.ToString()}
            };
            NotificationType notificationType;
            if (participant.UserRoleName.Contains(NotifyParams.Judge, StringComparison.InvariantCultureIgnoreCase) &&
               hearing.IsJudgeEmailEJud())
            {
                notificationType = NotificationType.HearingConfirmationEJudJudgeMultiDay;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (participant.UserRoleName.Contains(NotifyParams.StaffMember, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.HearingConfirmationStaffMemberMultiDay;
                parameters.Add(NotifyParams.StaffMember, $"{participant.FirstName} {participant.LastName}");
                parameters.Add(NotifyParams.UserName, participant.Username);
            }
            else if (participant.UserRoleName.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase) &&
                     !hearing.IsJudgeEmailEJud())
            {
                notificationType = NotificationType.HearingConfirmationJudgeMultiDay;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
                parameters.Add(NotifyParams.CourtroomAccountUserName, participant.Username);
                participant.ContactEmail = hearing.GetJudgeEmail();
                participant.TelephoneNumber = hearing.GetJudgePhone();
            }
            else if (participant.UserRoleName.Contains(RoleNames.JudicialOfficeHolder,
                StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = hearing.IsParticipantAEJudJudicialOfficeHolder(participant.Id) ? NotificationType.HearingConfirmationEJudJohMultiDay : NotificationType.HearingConfirmationJohMultiDay;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");
            }
            else if (participant.UserRoleName.Contains(RoleNames.Representative, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.HearingConfirmationRepresentativeMultiDay;
                parameters.Add(NotifyParams.ClientName, participant.Representee);
                parameters.Add(NotifyParams.SolicitorName, $"{participant.FirstName} {participant.LastName}");
            }
            else
            {
                notificationType = NotificationType.HearingConfirmationLipMultiDay;
                parameters.Add(NotifyParams.Name, $"{participant.FirstName} {participant.LastName}");
            }

            return new AddNotificationRequest
            {
                HearingId = hearing.Id,
                MessageType = MessageType.Email,
                ContactEmail = participant.ContactEmail,
                NotificationType = notificationType,
                ParticipantId = participant.Id,
                PhoneNumber = participant.TelephoneNumber,
                Parameters = parameters
            };
        }


        private static Dictionary<string, string> InitConfirmReminderParams(HearingDetailsResponse hearing)
        {
            var @case = hearing.Cases.First();
            return new Dictionary<string, string>
            {
                {NotifyParams.CaseName, @case.Name},
                {NotifyParams.CaseNumber, @case.Number},
                {NotifyParams.Time, hearing.ScheduledDateTime.ToEmailTimeGbLocale()},
                {NotifyParams.DayMonthYear, hearing.ScheduledDateTime.ToEmailDateGbLocale()}
            };
        }
    }
}