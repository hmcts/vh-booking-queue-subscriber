using System;
using System.Collections.Generic;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.UserApi;
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

            var notificationType = participant.UserRole.Contains(RoleNames.Individual, StringComparison.InvariantCultureIgnoreCase)
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

        public static AddNotificationRequest MapToNewHearingNotification(HearingDto hearing, ParticipantDto participant)
        {
            var parameters = InitHearingNotificationParams(hearing);
            var contactEmail = GetContactEmail(participant);

            NotificationType notificationType;
            if (participant.UserRole.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.HearingConfirmationEJudJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (participant.UserRole.Contains(RoleNames.JudicialOfficeHolder,
                StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = participant.IsParticipantAEJudJudicialOfficeHolder()
                    ? NotificationType.HearingConfirmationEJudJoh
                    : NotificationType.HearingConfirmationJoh;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");
            }
            else if (participant.UserRole.Contains(RoleNames.Representative, StringComparison.InvariantCultureIgnoreCase))
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
                HearingId = hearing.HearingId,
                MessageType = MessageType.Email,
                ContactEmail = contactEmail,
                NotificationType = notificationType,
                ParticipantId = participant.ParticipantId,
                PhoneNumber = participant.ContactTelephone,
                Parameters = parameters
            };
        }

        public static AddNotificationRequest MapToHearingAmendmentNotification(HearingDto hearing, ParticipantDto participant, DateTime originalDateTime,
            DateTime newDateTime)
        {
            var contactEmail = GetContactEmail(participant);

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
                ContactEmail = contactEmail,
                NotificationType = notificationType,
                ParticipantId = participant.ParticipantId,
                PhoneNumber = participant.ContactTelephone,
                Parameters = parameters
            };
        }

        public static AddNotificationRequest MapToMultiDayHearingConfirmationNotification(
            HearingDto hearing, ParticipantDto participant, int days)
        {
            var contactEmail = GetContactEmail(participant);

            var cleanedCaseName = hearing.CaseName.Replace($"Day 1 of {days}", string.Empty).Trim();
            var parameters = new Dictionary<string, string>
            {
                {NotifyParams.CaseName, cleanedCaseName},
                {NotifyParams.CaseNumber, hearing.CaseNumber},
                {NotifyParams.Time, hearing.ScheduledDateTime.ToEmailTimeGbLocale()},
                {NotifyParams.StartDayMonthYear, hearing.ScheduledDateTime.ToEmailDateGbLocale()},
                {NotifyParams.NumberOfDays, days.ToString()}
            };
            NotificationType notificationType;
            if (participant.UserRole.Contains(NotifyParams.Judge, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.HearingConfirmationEJudJudgeMultiDay;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (participant.UserRole.Contains(RoleNames.JudicialOfficeHolder,
                StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = participant.IsParticipantAEJudJudicialOfficeHolder() ? NotificationType.HearingConfirmationEJudJohMultiDay : NotificationType.HearingConfirmationJohMultiDay;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");
            }
            else if (participant.UserRole.Contains(RoleNames.Representative, StringComparison.InvariantCultureIgnoreCase))
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
                HearingId = hearing.HearingId,
                MessageType = MessageType.Email,
                ContactEmail = contactEmail,
                NotificationType = notificationType,
                ParticipantId = participant.ParticipantId,
                PhoneNumber = participant.ContactTelephone,
                Parameters = parameters
            };
        }

        public static AddNotificationRequest MapToDemoOrTestNotification(HearingDto hearing, ParticipantDto participant, string testType)
        {
            var contactEmail = GetContactEmail(participant);

            var parameters = new Dictionary<string, string>()
            {
                {NotifyParams.CaseNumber, hearing.CaseNumber},
                {NotifyParams.TestType,testType},
                {NotifyParams.Date,hearing.ScheduledDateTime.ToEmailDateGbLocale() },
                {NotifyParams.Time,hearing.ScheduledDateTime.ToEmailTimeGbLocale()},
                {NotifyParams.UserName,participant.Username.ToLower()}
            };

            NotificationType notificationType;
            if (participant.IsParticipantAEJudJudicialOfficeHolder())
            {
                notificationType = NotificationType.EJudJohDemoOrTest;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");
            }
            else if (participant.UserRole.Contains(RoleNames.StaffMember, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.StaffMemberDemoOrTest;
                parameters.Add(NotifyParams.StaffMember, $"{participant.FirstName} {participant.LastName}");
            }
            else if (participant.UserRole.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.EJudJudgeDemoOrTest;
                parameters.Add(NotifyParams.CourtroomAccountUserName, participant.Username);
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
                parameters.Remove(NotifyParams.UserName);
            }
            else
            {
                notificationType = NotificationType.ParticipantDemoOrTest;
                parameters.Add(NotifyParams.Name, $"{participant.FirstName} {participant.LastName}");
            }

            return new AddNotificationRequest
            {
                HearingId = hearing.HearingId,
                MessageType = MessageType.Email,
                ContactEmail = contactEmail,
                NotificationType = notificationType,
                ParticipantId = participant.ParticipantId,
                PhoneNumber = participant.ContactTelephone,
                Parameters = parameters
            };
        }

        private static Dictionary<string, string> InitHearingNotificationParams(HearingDto hearing)
        {   
            return new Dictionary<string, string>
            {
                {NotifyParams.CaseName, hearing.CaseName},
                {NotifyParams.CaseNumber, hearing.CaseNumber},
                {NotifyParams.Time, hearing.ScheduledDateTime.ToEmailTimeGbLocale()},
                {NotifyParams.DayMonthYear, hearing.ScheduledDateTime.ToEmailDateGbLocale()}
            };
        }
        private static string GetContactEmail(ParticipantDto participant)
        {
            var contactEmail = participant.ContactEmail;
            if (participant.UserRole.Equals(RoleNames.Judge, StringComparison.CurrentCultureIgnoreCase) && !participant.IsUsernameEjud() 
                && !string.IsNullOrEmpty(participant.ContactEmailForNonEJudJudgeUser))
            {
                contactEmail = participant.ContactEmailForNonEJudJudgeUser;
            }

            return contactEmail;
        }
    }
}