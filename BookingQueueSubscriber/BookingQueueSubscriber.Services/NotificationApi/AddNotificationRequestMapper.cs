using BookingQueueSubscriber.Services.UserApi;
using NotificationApi.Contract;
using NotificationApi.Contract.Requests;
using RoleNames = BookingQueueSubscriber.Services.UserApi.RoleNames;

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
        
        
        public static AddNotificationRequest MapToNewUserWelcomeEmail(HearingDto hearing, ParticipantDto participant)
        {
            var parameters = new Dictionary<string, string>
            {
                {NotifyParams.Name, $"{participant.FirstName} {participant.LastName}"},
                {NotifyParams.CaseName, hearing.CaseName},
                {NotifyParams.CaseNumber, hearing.CaseNumber}
            };
            
            var notificationType = participant.UserRole.Contains(RoleNames.Individual, StringComparison.InvariantCultureIgnoreCase)
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

        public static AddNotificationRequest MapToNewHearingNotification(HearingDto hearing, ParticipantDto participant)
        {
            var contactEmail = participant.ContactEmail;
            var contactTelephone = participant.ContactTelephone;
            var parameters = InitHearingNotificationParams(hearing);

            NotificationType notificationType;
            var isJudge = participant.UserRole.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase);
            
            if (isJudge && participant.HasEjdUsername())
            {
                notificationType = NotificationType.HearingConfirmationEJudJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (isJudge) // default to non ejud template for judges without ejud username
            {
                notificationType = NotificationType.HearingConfirmationJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
                parameters.Add(NotifyParams.CourtroomAccountUserName, participant.Username);
                contactEmail = GetContactEmailForNonEJudJudgeUser(participant) ?? participant.ContactEmail;
                contactTelephone = GetContactPhoneForNonEJudJudgeUser(participant) ?? participant.ContactTelephone;
            }
            else if (participant.UserRole.Contains(RoleNames.JudicialOfficeHolder, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = participant.HasEjdUsername() ? NotificationType.HearingConfirmationEJudJoh : NotificationType.HearingConfirmationJoh;
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
                PhoneNumber = contactTelephone,
                Parameters = parameters
            };
        }

        public static AddNotificationRequest MapToHearingAmendmentNotification(HearingDto hearing, ParticipantDto participant, DateTime originalDateTime, DateTime newDateTime)
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
            if (participant.UserRole.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase) && participant.HasEjdUsername())
            {
                notificationType = NotificationType.HearingAmendmentEJudJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (participant.UserRole.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = NotificationType.HearingAmendmentJudge;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
                parameters.Add(NotifyParams.CourtroomAccountUserName, participant.Username);
                contactEmail = GetContactEmailForNonEJudJudgeUser(participant) ?? participant.ContactEmail;
                contactTelephone = GetContactPhoneForNonEJudJudgeUser(participant) ?? participant.ContactTelephone;
            }
            else if (participant.UserRole.Contains(RoleNames.JudicialOfficeHolder, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = participant.HasEjdUsername() ? NotificationType.HearingAmendmentEJudJoh : NotificationType.HearingAmendmentJoh;
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
                PhoneNumber = contactTelephone,
                Parameters = parameters
            };
        }

        public static AddNotificationRequest MapToMultiDayHearingConfirmationNotification(
            HearingDto hearing, ParticipantDto participant, int days, bool usePostMay2023Template = false, string userPassword = null)
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
                {NotifyParams.NumberOfDays, days.ToString()},
            };
            
            NotificationType notificationType;
            
            var isJudge = participant.UserRole.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase);

            if (isJudge && participant.HasEjdUsername())
            {
                notificationType = NotificationType.HearingConfirmationEJudJudgeMultiDay;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
            }
            else if (isJudge)
            {
                notificationType = NotificationType.HearingConfirmationJudgeMultiDay;
                parameters.Add(NotifyParams.Judge, participant.DisplayName);
                parameters.Add(NotifyParams.CourtroomAccountUserName, participant.Username);
                contactEmail = GetContactEmailForNonEJudJudgeUser(participant) ?? participant.ContactEmail;
                contactTelephone = GetContactPhoneForNonEJudJudgeUser(participant) ?? participant.ContactTelephone;
            }
            else if (participant.UserRole.Contains(RoleNames.JudicialOfficeHolder, StringComparison.InvariantCultureIgnoreCase))
            {
                notificationType = participant.HasEjdUsername() ? NotificationType.HearingConfirmationEJudJohMultiDay : NotificationType.HearingConfirmationJohMultiDay;
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
                parameters = MapRequestForLipPostMay2023(participant, hearing, parameters, usePostMay2023Template, userPassword, out notificationType);
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

        private static Dictionary<string, string> MapRequestForLipPostMay2023(
            ParticipantDto participant, HearingDto hearing, Dictionary<string, string> parameters, bool usePostMay2023Template, string userPassword, out NotificationType notificationType)
        {
            if (usePostMay2023Template)
            {
                notificationType = NotificationType.NewUserLipConfirmationMultiDay;
                parameters.Add(NotifyParams.DayMonthYear, hearing.ScheduledDateTime.ToEmailDateGbLocale());
                parameters.Add(NotifyParams.DayMonthYearCy, hearing.ScheduledDateTime.ToEmailDateCyLocale());
                parameters.Add(NotifyParams.StartTime, hearing.ScheduledDateTime.ToEmailTimeGbLocale());
                parameters.Add(NotifyParams.UserName, participant.Username.ToLower());
                if (!string.IsNullOrEmpty(userPassword))
                {
                    parameters.Add(NotifyParams.RandomPassword, userPassword);
                }
                else
                {
                    notificationType = NotificationType.ExistingUserLipConfirmationMultiDay;
                }
            }
            else
            {
                notificationType = NotificationType.HearingConfirmationLipMultiDay;
            }
            parameters.Add(NotifyParams.Name, $"{participant.FirstName} {participant.LastName}");
            return parameters;
        }

        public static AddNotificationRequest MapToDemoOrTestNotification(HearingDto hearing, ParticipantDto participant, string testType)
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
            var isJudge = participant.UserRole.Contains(RoleNames.Judge, StringComparison.InvariantCultureIgnoreCase);
            var isJudicialOfficeHolder = participant.UserRole.Contains(RoleNames.JudicialOfficeHolder, StringComparison.InvariantCultureIgnoreCase);
            
            if (isJudicialOfficeHolder && participant.HasEjdUsername())
            {
                notificationType = NotificationType.EJudJohDemoOrTest;
                parameters.Add(NotifyParams.JudicialOfficeHolder, $"{participant.FirstName} {participant.LastName}");
            }
            else if (isJudge)
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
        
        public static AddNotificationRequest MapToNewUserAccountDetailsEmail(HearingDto hearing, ParticipantDto participant, string userPassword = null)
        {
            var contactEmail = participant.ContactEmail;
            var parameters = new Dictionary<string, string>()
            {
                {NotifyParams.Name, $"{participant.FirstName} {participant.LastName}" },
                {NotifyParams.CaseName, hearing.CaseName },
                {NotifyParams.CaseNumber, hearing.CaseNumber },
                
                {NotifyParams.DayMonthYear,hearing.ScheduledDateTime.ToEmailDateGbLocale() },
                {NotifyParams.DayMonthYearCy,hearing.ScheduledDateTime.ToEmailDateCyLocale() },
                
                {NotifyParams.StartTime,hearing.ScheduledDateTime.ToEmailTimeGbLocale() },
                {NotifyParams.UserName,participant.Username.ToLower() }
            };

            var notificationType = NotificationType.NewUserLipConfirmation;

            if (!string.IsNullOrEmpty(userPassword))
            {
                parameters.Add(NotifyParams.RandomPassword, userPassword);
            }
            else
            {
                notificationType = NotificationType.ExistingUserLipConfirmation;
            }
            
            
        
            var addNotificationRequest = new AddNotificationRequest
            {
                HearingId = hearing.HearingId,
                MessageType = MessageType.Email,
                ContactEmail = contactEmail,
                NotificationType = notificationType,
                ParticipantId = participant.ParticipantId,
                PhoneNumber = participant.ContactTelephone,
                Parameters = parameters
            };
            return addNotificationRequest;
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

        private static string GetContactEmailForNonEJudJudgeUser(ParticipantDto participant)
        {
            return participant.UserRole.Equals(RoleNames.Judge, StringComparison.CurrentCultureIgnoreCase) && !participant.HasEjdUsername()
                && !string.IsNullOrEmpty(participant.ContactEmailForNonEJudJudgeUser)
                ? participant.ContactEmailForNonEJudJudgeUser
                : null;
        }
  
        private static string GetContactPhoneForNonEJudJudgeUser(ParticipantDto participant)
        {
            return participant.UserRole.Equals(RoleNames.Judge, StringComparison.CurrentCultureIgnoreCase) && !participant.HasEjdUsername()
                && !string.IsNullOrEmpty(participant.ContactPhoneForNonEJudJudgeUser)
                ? participant.ContactPhoneForNonEJudJudgeUser
                : null;
        }
    }
}