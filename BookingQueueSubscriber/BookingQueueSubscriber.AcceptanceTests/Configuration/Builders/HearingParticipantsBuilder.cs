using System.Collections.Generic;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingQueueSubscriber.Services.BookingsApi;

namespace BookingQueueSubscriber.AcceptanceTests.Configuration.Builders
{
    public class HearingParticipantsBuilder
    {
        private readonly List<ParticipantRequest> _participants;
        private readonly string _usernameStem;

        public HearingParticipantsBuilder(string usernameStem)
        {
            _participants = new List<ParticipantRequest>();
            _usernameStem = usernameStem;
        }

        public HearingParticipantsBuilder AddJudge()
        {
            _participants.Add(AddParticipant("Judge"));
            return this;
        }

        public HearingParticipantsBuilder AddIndividual()
        {
            _participants.Add(AddParticipant("Individual"));
            return this;
        }

        public HearingParticipantsBuilder AddRep()
        {
            _participants.Add(AddParticipant("Representative"));
            return this;
        }

        public HearingParticipantsBuilder AddPanelMember()
        {
            _participants.Add(AddParticipant("Panel Member"));
            return this;
        }

        public HearingParticipantsBuilder AddObserver()
        {
            _participants.Add(AddParticipant("Observer"));
            return this;
        }

        public HearingParticipantsBuilder AddUser(string userType, int number)
        {
            _participants.Add(AddParticipant(userType, number));
            return this;
        }

        private ParticipantRequest AddParticipant(string userType, int number = 1)
        {
            var firstname = $"{UserData.AUTOMATED_FIRST_NAME_PREFIX}_{UserData.BOOKING_QUEUE_SUBSCRIBER_NAME_PREFIX}";
            var lastname = $"{userType} {number}";

            var participant = new ParticipantRequest()
            {
                AdditionalProperties = new Dictionary<string, object>(),
                Title = UserData.TITLE,
                First_name = firstname,
                Middle_names = UserData.MIDDLE_NAME,
                Last_name = lastname,
                Display_name =  $"{firstname} {lastname}",
                Contact_email = SetContactEmail(firstname, lastname),
                Username = SetUsername(firstname, lastname),
                Telephone_number = UserData.TELEPHONE_NUMBER,
                Case_role_name = userType,
                Hearing_role_name = userType,
                Representee = null,
                Organisation_name = null
            };

            if (userType.Equals("Individual"))
            {
                participant.Case_role_name = RoleData.CASE_ROLE_NAME;
                participant.Hearing_role_name = RoleData.INDV_HEARING_ROLE_NAME;
            }

            if (userType.Equals("Representative"))
            {
                participant.Case_role_name = RoleData.CASE_ROLE_NAME;
                participant.Hearing_role_name = RoleData.REPRESENTATIVE_HEARING_ROLE_NAME;
                participant.Representee = "Individual";
                participant.Organisation_name = UserData.ORGANISATION;
            }

            return participant;
        }

        private string SetUsername(string firstname, string lastname)
        {
            return $"{ReplaceSpacesWithUnderscores(firstname)}.{ReplaceSpacesWithUnderscores(lastname)}@{_usernameStem}"
                .ToLowerInvariant();
        }

        private string SetContactEmail(string firstname, string lastname)
        {
            return $"{ReplaceSpacesWithUnderscores(firstname)}.{ReplaceSpacesWithUnderscores(lastname)}@{ContactEmailStem(_usernameStem)}".ToLowerInvariant();
        }

        private static string ReplaceSpacesWithUnderscores(string text)
        {
            return text.Replace(" ", "_");
        }

        private static string ContactEmailStem(string emailStem)
        {
            return emailStem.Substring(16);
        }

        public List<ParticipantRequest> Build()
        {
            return _participants;
        }
    }
}
