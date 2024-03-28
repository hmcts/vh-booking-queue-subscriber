using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using NotificationApi.Contract;

namespace BookingQueueSubscriber.UnitTests.Mappers.NotificationMappers
{
    public class MapToMultiDayHearingConfirmationNotificationTests
    {
        [Test]
        public void should_map_to_ejud_judge_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationEJudJudgeMultiDay;
            var participant = GetParticipantDto("Judge");
            var hearing = GetHearingDto();

            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judicial office holder");
            expectedParameters.Add("judge", participant.DisplayName);

            var result = AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, 4, true);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_an_ejud_joh__multi_day_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationEJudJohMultiDay;
            var participant = GetParticipantDto("Judicial Office Holder");
            var hearing = GetHearingDto();

            var expectedParameters = GetExpectedParameters(hearing, participant);

            var result = AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, 4, true);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_judge_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationJudgeMultiDay;
            var participant = GetParticipantDto("Judge");
            participant.Username = "user@test.com";
            participant.ContactEmailForNonEJudJudgeUser = "judge@hmcts.net";
            participant.ContactPhoneForNonEJudJudgeUser = "123456789";
            var hearing = GetHearingDto();

            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judicial office holder");
            expectedParameters.Add("judge", participant.DisplayName);
            expectedParameters.Add("courtroom account username", participant.Username);
            var result = AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, 4, false);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmailForNonEJudJudgeUser);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactPhoneForNonEJudJudgeUser);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_lip_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationLipMultiDay;
            var participant = GetParticipantDto("Individual");
            var hearing = GetHearingDto();

            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judicial office holder");
            expectedParameters.Add("name", $"{participant.FirstName} {participant.LastName}");

            var result = AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, 4, false);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }
        
        [Test]
        public void should_map_to_new_lip_confirmation_notification_when_new_template_on()
        {
            var expectedNotificationType = NotificationType.NewUserLipConfirmationMultiDay;
            var participant = GetParticipantDto("Individual");
            var hearing = GetHearingDto();

            var expectedParameters = GetExpectedParametersForPostMay2023(hearing, participant);
            expectedParameters.Add("random password", "xyz");

            var result = AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(
                hearing, 
                participant, 
                4, 
                true, 
                "xyz");

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_representative_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationRepresentativeMultiDay;
            var participant = GetParticipantDto("Representative", "Jane Doe");
            var hearing = GetHearingDto();

            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judicial office holder");
            expectedParameters.Add("client name", $"{participant.Representee}");
            expectedParameters.Add("solicitor name", $"{participant.FirstName} {participant.LastName}");
            var result = AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, 4, false);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_existing_lip_confirmation_notification_when_new_template_on()
        {
            var expectedNotificationType = NotificationType.ExistingUserLipConfirmationMultiDay;
            var participant = GetParticipantDto("Individual");
            participant.Username = "user@test.com";

            var hearing = GetHearingDto();
            Dictionary<string, string> expectedParameters = GetExpectedParametersForPostMay2023(hearing, participant);

            var result = AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(
                hearing, 
                participant, 
                4,
                true);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        private static Dictionary<string, string> GetExpectedParameters(HearingDto hearing, ParticipantDto participant)
        {
            return new Dictionary<string, string>
            {
                {"case name", hearing.CaseName},
                {"case number", hearing.CaseNumber},
                {"time", "2:10 PM"},
                {"Start Day Month Year", "12 October 2020"},
                {"judicial office holder", $"{participant.FirstName} {participant.LastName}"},
                {"number of days", "4"}
            };
        }
        
        private static Dictionary<string, string> GetExpectedParametersForPostMay2023(HearingDto hearing, ParticipantDto participant)
        {
            return new Dictionary<string, string>
            {
                {"name", $"{participant.FirstName} {participant.LastName}"},
                {"case name", hearing.CaseName},
                {"case number", hearing.CaseNumber},
                {"number of days", "4"},
                {"Start Day Month Year", "12 October 2020"},
                {"day month year", "12 October 2020"},
                {"day month year_CY", "12 Hydref 2020"},
                {"username", participant.Username},
                {"start time", "2:10 PM"},
                {"time", "2:10 PM"}
            };
        }

        private static HearingDto GetHearingDto()
        {
            return new HearingDto
            {
                HearingId = Guid.NewGuid(),
                ScheduledDateTime = new DateTime(2020, 10, 12, 13, 10, 0, DateTimeKind.Utc),
                CaseName = "1234",
                CaseNumber = "MBFY/17364",
                CaseType = "test"
            };
        }

        private static ParticipantDto GetParticipantDto(string userRole, string representee = null)
        {
            return new ParticipantDto
            {
                ParticipantId = Guid.NewGuid(),
                Username = "contact@judiciary.hmcts.net",
                ContactEmail = "contact@hmcts.net",
                FirstName = "John",
                HearingRole = "hearingrolename",
                LastName = "Doe",
                ContactTelephone = "0123456789",
                UserRole = userRole,
                DisplayName = "Johnny",
                Representee = representee
            };
        }
    }
}