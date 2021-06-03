using System.Collections.Generic;
using System.Linq;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using Castle.Core.Internal;
using FluentAssertions;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.AcceptanceTests.Tests
{
    public static class Verify
    {
        public static void ConferenceDetailsResponse(ConferenceDetailsResponse response, HearingDetailsResponse hearing)
        {
            response.AudioRecordingRequired.Should().Be(hearing.AudioRecordingRequired);
            response.CaseName.Should().Be(hearing.Cases.First().Name);
            response.CaseNumber.Should().Be(hearing.Cases.First().Number);
            response.CaseType.Should().Be(hearing.CaseTypeName);
            response.ClosedDateTime.Should().BeNull();
            response.CurrentStatus.Should().Be(ConferenceState.NotStarted);
            response.HearingId.Should().Be(hearing.Id);
            response.HearingVenueName.Should().Be(hearing.HearingVenueName);
            response.Id.Should().NotBeEmpty();
            response.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
            response.ScheduledDuration.Should().Be(hearing.ScheduledDuration);
            response.StartedDateTime.Should().BeNull();
            VerifyConferenceParticipants(response.Participants, hearing.Participants);
        }

        private static void VerifyConferenceParticipants(IReadOnlyCollection<ParticipantDetailsResponse> hearingParticipants,
            IReadOnlyCollection<ParticipantResponse> conferenceParticipants)
        {
            hearingParticipants.Count.Should().Be(conferenceParticipants.Count);
            foreach (var hearingParticipant in hearingParticipants)
            {
                var conferenceParticipant =
                    conferenceParticipants.First(x => x.LastName.Equals(hearingParticipant.LastName));

                conferenceParticipant.CaseRoleName.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.ContactEmail.Should().Be(hearingParticipant.ContactEmail);
                conferenceParticipant.DisplayName.Should().Be(hearingParticipant.DisplayName);
                conferenceParticipant.FirstName.Should().Be(hearingParticipant.FirstName);
                conferenceParticipant.HearingRoleName.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Id.Should().NotBeEmpty();
                conferenceParticipant.MiddleNames.Should().BeNullOrWhiteSpace();
                conferenceParticipant.LastName.Should().Be(hearingParticipant.LastName);
                conferenceParticipant.TelephoneNumber.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Title.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Username.Should().Be(hearingParticipant.Username);

                if (conferenceParticipant.LinkedParticipants.Any())
                { 
                   var linkedParticipant = conferenceParticipant.HearingRoleName != RoleData.INTERPRETER_HEARING_ROLE_NAME ? 
                                            hearingParticipants.FirstOrDefault(c => c.HearingRole == RoleData.INTERPRETER_HEARING_ROLE_NAME)
                                            : hearingParticipants.FirstOrDefault(c => c.HearingRole != RoleData.INTERPRETER_HEARING_ROLE_NAME && c.LinkedParticipants.Any());
                    
                    hearingParticipant.LinkedParticipants.Any().Should().BeTrue();
                    conferenceParticipant.LinkedParticipants.Any(c => c.LinkedId == linkedParticipant.RefId).Should().BeTrue();
                }

                if (!conferenceParticipant.UserRoleName.Equals("Representative") ||
                    conferenceParticipant.CaseRoleName.ToLower() == "none") continue;
                conferenceParticipant.Organisation.Should().NotBeNullOrWhiteSpace();


            }
        }

        public static void UpdatedHearing(HearingDetailsResponse hearingDetails, UpdateHearingRequest request)
        {
            hearingDetails.Should().BeEquivalentTo(request);
        }

        public static void UpdatedConference(ConferenceDetailsResponse conferenceDetails, UpdateHearingRequest request)
        {
            conferenceDetails.Should().BeEquivalentTo(request, options => options
                .Excluding(x => x.Cases)
                .Excluding(x => x.HearingRoomName)
                .Excluding(x => x.OtherInformation)
                .Excluding(x => x.QuestionnaireNotRequired)
                .Excluding(x => x.UpdatedBy)
            );

            conferenceDetails.CaseName.Should().Be(request.Cases.First().Name);
            conferenceDetails.CaseNumber.Should().Be(request.Cases.First().Number);
        }

        public static void ParticipantDetails(ParticipantDetailsResponse participant, AddParticipantsToHearingRequest request)
        {
            participant.Should().BeEquivalentTo(request.Participants.First(),
                options => options.ExcludingMissingMembers().Excluding(x => x.Representee));
        }
    }
}
