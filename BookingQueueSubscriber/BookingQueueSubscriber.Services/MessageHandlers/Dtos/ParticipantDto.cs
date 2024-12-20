﻿using BookingQueueSubscriber.Services.UserApi;

namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class ParticipantDto
    {
        public Guid ParticipantId { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string DisplayName { get; set; }
        public string HearingRole { get; set; }
        public string UserRole { get; set; }
        public string Representee { get; set; }

        public IList<LinkedParticipantDto> LinkedParticipants { get; set; }
        public string ContactEmailForNonEJudJudgeUser { get; set; }
        public string ContactPhoneForNonEJudJudgeUser { get; set; }
        public bool SendHearingNotificationIfNew { get; set; }

        public bool IsIndividual() => UserRole.Equals(RoleNames.Individual, StringComparison.CurrentCultureIgnoreCase);
    }
}