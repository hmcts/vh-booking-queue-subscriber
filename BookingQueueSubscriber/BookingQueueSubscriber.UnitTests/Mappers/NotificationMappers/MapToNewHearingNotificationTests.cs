using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using NotificationApi.Contract;

namespace BookingQueueSubscriber.UnitTests.Mappers.NotificationMappers
{
    public class MapToNewHearingNotificationTests
    {
        [Test]
        public void Should_map_to_ejud_judge_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationEJudJudge;
            var participant = GetParticipantDto("Judge");
            participant.ContactEmail = "user@judiciarytest.com";
            var hearing = GetHearingDto();

            var expectedParameters = GetExpectedParameters(hearing, participant);

            var result = AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant);

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
        public void Should_map_to_ejud_joh_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationEJudJoh;
            var participant = GetParticipantDto("Judicial Office Holder");
            var hearing = GetHearingDto();

            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judge");
            expectedParameters.Add("judicial office holder", $"{participant.FirstName} {participant.LastName}");

            var result = AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant);

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
        public void Should_map_to_judge_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationJudge;
            const string expectedJudgeEmail = "judge@hmcts.net";
            const string expectedPhone = "123456789";
            var participant = GetParticipantDto("Judge");
            participant.Username = "judge@hearings.net";
            participant.ContactEmailForNonEJudJudgeUser = expectedJudgeEmail;
            participant.ContactPhoneForNonEJudJudgeUser = expectedPhone;
            var hearing = GetHearingDto();

            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Add("courtroom account username", participant.Username);

            var result = AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(expectedJudgeEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(expectedPhone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void Should_map_to_judge_confirmation_notification_has_other_contact_email()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationJudge;
            const string expectedPhone = "123456789";
            var participant = GetParticipantDto("Judge");
            participant.Username = "judge@hearings.net";
            participant.ContactPhoneForNonEJudJudgeUser = expectedPhone;
            var hearing = GetHearingDto();

            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Add("courtroom account username", participant.Username);

            var result = AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(expectedPhone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void Should_map_to_lip_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationLip;
            var participant = GetParticipantDto("Individual");
            var hearing = GetHearingDto();
            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judge");
            expectedParameters.Add("name", $"{participant.FirstName} {participant.LastName}");

            var result = AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant);

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
        public void Should_map_to_representative_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationRepresentative;
            var participant = GetParticipantDto("Representative");
            participant.Representee = "test";
            var hearing = GetHearingDto();
            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judge");
            expectedParameters.Add("solicitor name", $"{participant.FirstName} {participant.LastName}");
            expectedParameters.Add("client name", $"{participant.Representee}");
            
            var result = AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant);

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
        public void Should_map_to_joh_confirmation_notification()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationJoh;
            var participant = GetParticipantDto("Judicial Office Holder");
            participant.Username = "joh@hearings.net";
            var hearing = GetHearingDto();
            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judge");
            expectedParameters.Add("judicial office holder", $"{participant.FirstName} {participant.LastName}");

            var result = AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant);

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
        public void Should_map_to_joh_confirmation_notification_has_ejud_username()
        {
            var expectedNotificationType = NotificationType.HearingConfirmationEJudJoh;
            var participant = GetParticipantDto("Judicial Office Holder");
            var hearing = GetHearingDto();
            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judge");
            expectedParameters.Add("judicial office holder", $"{participant.FirstName} {participant.LastName}");

            var result = AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant);

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
                {"case name", "1234"},
                {"case number", hearing.CaseNumber},
                {"time", "12:15 PM"},
                {"day month year", "10 February 2020"},
                {"judge", participant.DisplayName}
            };
        }

        private static ParticipantDto GetParticipantDto(string userRole)
        {
            return new ParticipantDto
            {
                ParticipantId = Guid.NewGuid(),
                Username = "contact@judiciary.hmcts.net",
                ContactEmail = "judge@judiciary.hmcts.net",
                FirstName = "John",
                HearingRole = "hearingrolename",
                LastName = "Doe",
                ContactTelephone = "0123456789",
                UserRole = userRole,
                DisplayName = "Johnny",
            };
        }

        private static HearingDto GetHearingDto()
        {
            return new HearingDto
            {
                HearingId = Guid.NewGuid(),
                ScheduledDateTime = new DateTime(2020, 2, 10, 12, 15, 0, DateTimeKind.Utc),
                CaseName = "1234",
                CaseNumber = "MBFY/17364",
                CaseType = "test"
            };
        }
    }
}