using System.Collections.Generic;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingsApi.Contract.Requests;

namespace BookingQueueSubscriber.AcceptanceTests.Configuration.Builders
{
    public class HearingParticipantsBuilder
    {
        private readonly List<ParticipantRequest> _participants;
        private readonly string _usernameStem;
        private bool _isCacdHearing;

        public HearingParticipantsBuilder(string usernameStem, bool isCacdHearing)
        {
            _isCacdHearing = isCacdHearing;
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

        public HearingParticipantsBuilder AddWinger()
        {
            _participants.Add(AddParticipant("Winger"));
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
                Title = UserData.TITLE,
                FirstName = firstname,
                MiddleNames = UserData.MIDDLE_NAME,
                LastName = lastname,
                DisplayName =  $"{firstname} {lastname}",
                ContactEmail = SetContactEmail(firstname, lastname),
                Username = SetUsername(firstname, lastname),
                TelephoneNumber = UserData.TELEPHONE_NUMBER,
                CaseRoleName = userType,
                HearingRoleName = userType,
                Representee = null,
                OrganisationName = null
            };

            if (userType.Equals("Individual"))
            {
                participant.CaseRoleName = _isCacdHearing ? RoleData.CACD_CASE_ROLE_NAME : RoleData.CASE_ROLE_NAME;
                participant.HearingRoleName = _isCacdHearing ? RoleData.APPELLANT_CASE_ROLE_NAME : RoleData.INDV_HEARING_ROLE_NAME;
            }

            if (userType.Equals("Representative"))
            {
                participant.CaseRoleName = _isCacdHearing ? RoleData.CACD_CASE_ROLE_NAME : RoleData.CASE_ROLE_NAME;
                participant.HearingRoleName = _isCacdHearing ? RoleData.CACD_REP_HEARING_ROLE_NAME : RoleData.REPRESENTATIVE_HEARING_ROLE_NAME;
                participant.Representee = "Individual";
                participant.OrganisationName = UserData.ORGANISATION;
            }

            if (userType.Equals("Winger"))
            {
                participant.CaseRoleName = _isCacdHearing ? RoleData.CACD_CASE_ROLE_NAME : RoleData.WINGER_ROLE_NAME;
                participant.HearingRoleName =
                    _isCacdHearing ? RoleData.CACD_REP_HEARING_ROLE_NAME : RoleData.WINGER_ROLE_NAME;
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
