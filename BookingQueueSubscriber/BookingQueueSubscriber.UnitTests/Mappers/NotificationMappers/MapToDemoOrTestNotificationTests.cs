using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using FluentAssertions;
using NotificationApi.Contract;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BookingQueueSubscriber.UnitTests.Mappers.NotificationMappers
{
    public class MapToDemoOrTestNotificationTests
    {
        [Test]
        public void Should_map_ejud_judge_demo_or_test_notification()
        {
            HearingDto hearing = GetHearingDto();
            const NotificationType expectedNotificationType = NotificationType.EJudJudgeDemoOrTest;
            const string testType = "Generic";
            ParticipantDto participant = GetParticipantDto("Judge");
            Dictionary<string, string> expectedParameters = GetExpectedParameters(hearing, participant);

            //Act
            var result = AddNotificationRequestMapper.MapToDemoOrTestNotification(hearing, participant, testType);

            //Assert
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
        public void Should_return_null_and_not_map_judge_demo_or_test_notification()
        {
            //Arrange
            var hearing = GetHearingDto();
            const string testType = "Generic";
            var participant = GetParticipantDto("Judge");
            participant.Username = "testusername@hmcts.net";
            participant.ContactEmail = "contact@hearings.reform.hmcts.net";
            //Act
            var result = AddNotificationRequestMapper.MapToDemoOrTestNotification(hearing, participant, testType);

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public void Should_map_judge_demo_or_test_notification_with_judge_email()
        {
            //Arrange
            const string expectedJudgeEmail = "judge@hmcts.net";
            var hearing = GetHearingDto();
            const NotificationType expectedNotificationType = NotificationType.JudgeDemoOrTest;
            const string testType = "Generic";

            var participant = GetParticipantDto("Judge");
            participant.ContactEmailForNonEJudJudgeUser = expectedJudgeEmail;
            participant.Username = "testusername@hmcts.net";

            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Add("courtroom account username", participant.Username);

            //Act
            var result = AddNotificationRequestMapper.MapToDemoOrTestNotification(hearing, participant, testType);

            //Assert
            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(expectedJudgeEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void Should_map_ejud_joh_demo_or_test_notification()
        {
            //Arrange
            var hearing = GetHearingDto();
            const NotificationType expectedNotificationType = NotificationType.EJudJohDemoOrTest;
            const string testType = "Generic";

            var participant = GetParticipantDto("Judicial Office Holder");
            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judge");
            expectedParameters.Add("judicial office holder", $"{ participant.FirstName} { participant.LastName}");
            expectedParameters.Add("username", $"{participant.Username.ToLower()}");

            //Act
            var result = AddNotificationRequestMapper.MapToDemoOrTestNotification(hearing, participant, testType);

            //Assert
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
        public void Should_map_participants_demo_or_test_notification()
        {
            //Arrange
            var hearing = GetHearingDto();
            const NotificationType expectedNotificationType = NotificationType.ParticipantDemoOrTest;
            const string testType = "Generic";

            var participant = GetParticipantDto("Representative");
            var expectedParameters = GetExpectedParameters(hearing, participant);
            expectedParameters.Remove("judge");
            expectedParameters.Add("name", $"{participant.FirstName} {participant.LastName}");
            expectedParameters.Add("username", $"{participant.Username.ToLower()}");

            //Act
            var result = AddNotificationRequestMapper.MapToDemoOrTestNotification(hearing, participant, testType);

            //Assert
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
                {"case number", hearing.CaseNumber},
                {"test type", "Generic"},
                {"date", "10 February 2020"},
                {"time", "12:15 PM"},
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
