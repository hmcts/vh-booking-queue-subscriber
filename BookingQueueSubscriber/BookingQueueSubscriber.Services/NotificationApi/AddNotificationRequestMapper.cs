using BookingQueueSubscriber.Services.UserApi;
using NotificationApi.Contract;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    public static class AddNotificationRequestMapper
    {
        /// <summary>
        /// Maps to the <see cref="NotificationType.CreateIndividual"/> or <see cref="NotificationType.CreateRepresentative"/> notification type
        /// </summary>
        /// <param name="hearingId"></param>
        /// <param name="participant"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static AddNotificationRequest MapToNewUserNotification(Guid hearingId, ParticipantDto participant, string password)
        {
            var parameters = new Dictionary<string, string>
            {
                {NotifyParams.Name, $"{participant.FirstName} {participant.LastName}"},
                {NotifyParams.UserName, $"{participant.Username}"},
                {NotifyParams.RandomPassword, $"{password}"}
            };

            var notificationType = participant.IsIndividual()
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
        
        /// <summary>
        /// Maps to the <see cref="NotificationType.NewUserLipWelcome"/>
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static AddNotificationRequest MapToNewUserWelcomeEmail(HearingDto hearing, ParticipantDto participant)
        {
            var parameters = new Dictionary<string, string>
            {
                {NotifyParams.Name, $"{participant.FirstName} {participant.LastName}"},
                {NotifyParams.CaseName, hearing.CaseName},
                {NotifyParams.CaseNumber, hearing.CaseNumber}
            };
            
            var notificationType = participant.IsIndividual()
                ? NotificationType.NewUserLipWelcome
                : throw new NotSupportedException($"Only {RoleNames.Individual} is supported for {nameof(MapToNewUserWelcomeEmail)}. Current participant is {participant.UserRole}");
            
            var addNotificationRequest = new AddNotificationRequest
            {
                HearingId = hearing.HearingId,
                MessageType = MessageType.Email,
                ContactEmail = participant.ContactEmail,
                NotificationType = notificationType,
                ParticipantId = participant.ParticipantId,
                PhoneNumber = participant.ContactTelephone,
                Parameters = parameters
            };
            return addNotificationRequest;
        }

        /// <summary>
        /// Maps to HearingConfirmation for respective participant type
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <param name="eJudFeatureEnabled"></param>
        /// <returns></returns>
        public static AddNotificationRequest MapToNewHearingConfirmationNotification(HearingDto hearing, ParticipantDto participant, bool eJudFeatureEnabled)
        {
            var contactEmail = participant.ContactEmail;
            var contactTelephone = participant.ContactTelephone;
            var parameters = InitHearingNotificationParams(hearing);

            NotificationType notificationType;
            
            if (participant.IsJudge() && eJudFeatureEnabled && participant.HasEjdUsername())
            {
                notificationType = NotificationType.HearingConfirmationEJudJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (participant.IsJudge()) // default to non ejud template for judges without ejud username
            {
                notificationType = NotificationType.HearingConfirmationJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
                parameters.Add(NotifyParams.CourtroomAccountUserName, participant.Username);
                contactEmail = GetContactEmailForNonEJudJudgeUser(participant) ?? participant.ContactEmail;
                contactTelephone = GetContactPhoneForNonEJudJudgeUser(participant) ?? participant.ContactTelephone;
            }
            else if (participant.IsJudicialOfficeHolder())
            {
                notificationType = eJudFeatureEnabled && participant.HasEjdUsername() ? NotificationType.HearingConfirmationEJudJoh : NotificationType.HearingConfirmationJoh;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");
            }
            else if (participant.IsRepresentative())
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
                PhoneNumber = contactTelephone,
                Parameters = parameters
            };
        }

        /// <summary>
        /// Maps to hearing amendment notification for respective participant type
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <param name="originalDateTime"></param>
        /// <param name="newDateTime"></param>
        /// <param name="eJudFeatureEnabled"></param>
        /// <returns></returns>
        public static AddNotificationRequest MapToHearingAmendmentNotification(HearingDto hearing, ParticipantDto participant, DateTime originalDateTime,
            DateTime newDateTime, bool eJudFeatureEnabled)
        {
            var contactEmail = participant.ContactEmail;
            var contactTelephone = participant.ContactTelephone;
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
            if (participant.IsJudge() && eJudFeatureEnabled && participant.HasEjdUsername())
            {
                notificationType = NotificationType.HearingAmendmentEJudJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (participant.IsJudge() &&
                     !eJudFeatureEnabled)
            {
                notificationType = NotificationType.HearingAmendmentJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
                parameters.Add(NotifyParams.CourtroomAccountUserName, participant.Username);
                contactEmail = GetContactEmailForNonEJudJudgeUser(participant) ?? participant.ContactEmail;
                contactTelephone = GetContactPhoneForNonEJudJudgeUser(participant) ?? participant.ContactTelephone;
            }
            else if (participant.IsJudicialOfficeHolder())
            {
                notificationType = eJudFeatureEnabled && participant.HasEjdUsername() ? NotificationType.HearingAmendmentEJudJoh : NotificationType.HearingAmendmentJoh;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");

            }
            else if (participant.IsRepresentative())
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
                PhoneNumber = contactTelephone,
                Parameters = parameters
            };
        }

        /// <summary>
        /// Map to hearing confirmation for multi day hearing for respective participant type
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <param name="days"></param>
        /// <param name="eJudFeatureEnabled"></param>
        /// <returns></returns>
        public static AddNotificationRequest MapToMultiDayHearingConfirmationNotification(
            HearingDto hearing, ParticipantDto participant, int days, bool eJudFeatureEnabled)
        {
            var contactEmail = participant.ContactEmail;
            var contactTelephone = participant.ContactTelephone;
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
            

            if (participant.IsJudge() && eJudFeatureEnabled && participant.HasEjdUsername())
            {
                notificationType = NotificationType.HearingConfirmationEJudJudgeMultiDay;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (participant.IsJudge())
            {
                notificationType = NotificationType.HearingConfirmationJudgeMultiDay;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
                parameters.Add(NotifyParams.CourtroomAccountUserName, participant.Username);
                contactEmail = GetContactEmailForNonEJudJudgeUser(participant) ?? participant.ContactEmail;
                contactTelephone = GetContactPhoneForNonEJudJudgeUser(participant) ?? participant.ContactTelephone;
            }
            else if (participant.IsJudicialOfficeHolder())
            {
                notificationType = eJudFeatureEnabled && participant.HasEjdUsername() ? NotificationType.HearingConfirmationEJudJohMultiDay : NotificationType.HearingConfirmationJohMultiDay;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");
            }
            else if (participant.IsRepresentative())
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
                PhoneNumber = contactTelephone,
                Parameters = parameters
            };
        }

        public static AddNotificationRequest MapToDemoOrTestNotification(HearingDto hearing, ParticipantDto participant, string testType, bool eJudFeatureEnabled)
        {
            var contactEmail = participant.ContactEmail;
            var parameters = new Dictionary<string, string>()
            {
                {NotifyParams.CaseNumber, hearing.CaseNumber},
                {NotifyParams.TestType,testType},
                {NotifyParams.Date,hearing.ScheduledDateTime.ToEmailDateGbLocale() },
                {NotifyParams.Time,hearing.ScheduledDateTime.ToEmailTimeGbLocale()},
                {NotifyParams.UserName,participant.Username.ToLower()}
            };

            NotificationType notificationType;
            
            if (participant.IsJudicialOfficeHolder() && eJudFeatureEnabled && participant.HasEjdUsername())
            {
                notificationType = NotificationType.EJudJohDemoOrTest;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");
            }
            else if (participant.IsJudge())
            {
                var contactEmailForNonEJudJudgeUser = GetContactEmailForNonEJudJudgeUser(participant);
                bool isEmailEjud = participant.HasEjdUsername();
                if (string.IsNullOrEmpty(contactEmailForNonEJudJudgeUser) && !isEmailEjud)
                {
                    return null;
                }
                if (isEmailEjud)
                {
                    notificationType = NotificationType.EJudJudgeDemoOrTest;
                }
                else
                {
                    notificationType = NotificationType.JudgeDemoOrTest;
                    contactEmail = contactEmailForNonEJudJudgeUser;
                    parameters.Add(NotifyParams.CourtroomAccountUserName, participant.Username);
                }

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

        public static AddNotificationRequest MapToPostMay2023NewUserHearingConfirmationNotification(HearingDto hearing,
            ParticipantDto participant, string password)
        {
            if (!participant.IsIndividual())
            {
                throw new InvalidOperationException("Only individual participants are supported for this notification");
            }
            
            var parameters = InitHearingNotificationParams(hearing);
            parameters.Add(NotifyParams.Name, $"{participant.FirstName} {participant.LastName}");
            parameters.Add(NotifyParams.StartTime, hearing.ScheduledDateTime.ToEmailTimeGbLocale());
            parameters.Add(NotifyParams.UserName, participant.Username.ToLower());
            parameters.Add(NotifyParams.RandomPassword, password);

            var notificationType = NotificationType.NewUserLipConfirmation;

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
        
        public static AddNotificationRequest MapToPostMay2023ExistingUserHearingConfirmationNotification(HearingDto hearing, ParticipantDto participant)
        {
            if (!participant.IsIndividual())
            {
                throw new InvalidOperationException("Only individual participants are supported for this notification");
            }
            
            var parameters = InitHearingNotificationParams(hearing);
            parameters.Add(NotifyParams.Name, $"{participant.FirstName} {participant.LastName}");
            parameters.Add(NotifyParams.StartTime, hearing.ScheduledDateTime.ToEmailTimeGbLocale());
            parameters.Add(NotifyParams.UserName, participant.Username.ToLower());
            
            var notificationType = NotificationType.ExistingUserLipConfirmation;

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
        
        public static AddNotificationRequest MapToPostMay2023NewUserMultiDayHearingConfirmationNotification(HearingDto hearing,
            ParticipantDto participant, string password, int totalDays)
        {
            if (!participant.IsIndividual())
            {
                throw new InvalidOperationException("Only individual participants are supported for this notification");
            }
            
            var parameters = InitHearingNotificationParams(hearing);
            parameters.Add(NotifyParams.Name, $"{participant.FirstName} {participant.LastName}");
            parameters.Add(NotifyParams.StartTime, hearing.ScheduledDateTime.ToEmailTimeGbLocale());
            parameters.Add(NotifyParams.NumberOfDays, totalDays.ToString());
            parameters.Add(NotifyParams.UserName, participant.Username.ToLower());
            parameters.Add(NotifyParams.RandomPassword, password);

            var notificationType = NotificationType.NewUserLipConfirmationMultiDay;
            
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
        
        public static AddNotificationRequest MapToPostMay2023ExistingUserMultiHearingConfirmationNotification(HearingDto hearing,
            ParticipantDto participant, int totalDays)
        {
            if (!participant.IsIndividual())
            {
                throw new InvalidOperationException("Only individual participants are supported for this notification");
            }
            
            var parameters = InitHearingNotificationParams(hearing);

            var notificationType = NotificationType.ExistingUserLipConfirmationMultiDay;
            parameters.Add(NotifyParams.Name, $"{participant.FirstName} {participant.LastName}");
            parameters.Add(NotifyParams.StartTime, hearing.ScheduledDateTime.ToEmailTimeGbLocale());
            parameters.Add(NotifyParams.NumberOfDays, totalDays.ToString());
            parameters.Add(NotifyParams.UserName, participant.Username.ToLower());
            
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
        
        private static Dictionary<string, string> InitHearingNotificationParams(HearingDto hearing)
        {   
            return new Dictionary<string, string>
            {
                {NotifyParams.CaseName, hearing.CaseName},
                {NotifyParams.CaseNumber, hearing.CaseNumber},
                {NotifyParams.Time, hearing.ScheduledDateTime.ToEmailTimeGbLocale()},
                {NotifyParams.DayMonthYear, hearing.ScheduledDateTime.ToEmailDateGbLocale()},
                {NotifyParams.DayMonthYearCy, hearing.ScheduledDateTime.ToEmailDateCyLocale()}
            };
        }

        private static string GetContactEmailForNonEJudJudgeUser(ParticipantDto participant)
        {
            return participant.IsJudge() && !participant.HasEjdUsername()
                && !string.IsNullOrEmpty(participant.ContactEmailForNonEJudJudgeUser)
                ? participant.ContactEmailForNonEJudJudgeUser
                : null;
        }
  
        private static string GetContactPhoneForNonEJudJudgeUser(ParticipantDto participant)
        {
            return participant.IsJudge() && !participant.HasEjdUsername()
                && !string.IsNullOrEmpty(participant.ContactPhoneForNonEJudJudgeUser)
                ? participant.ContactPhoneForNonEJudJudgeUser
                : null;
        }

    }
}