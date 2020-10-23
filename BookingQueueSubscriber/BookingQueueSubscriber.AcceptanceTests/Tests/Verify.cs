using System.Collections.Generic;
using System.Linq;
using BookingQueueSubscriber.Services.BookingsApi;
using BookingQueueSubscriber.Services.VideoApi;
using FluentAssertions;

namespace BookingQueueSubscriber.AcceptanceTests.Tests
{
    public static class Verify
    {
        public static void ConferenceDetailsResponse(ConferenceDetailsResponse response, HearingDetailsResponse hearing)
        {
            response.Audio_recording_required.Should().Be(hearing.Audio_recording_required);
            response.Case_name.Should().Be(hearing.Cases.First().Name);
            response.Case_number.Should().Be(hearing.Cases.First().Number);
            response.Case_type.Should().Be(hearing.Case_type_name);
            response.Closed_date_time.Should().BeNull();
            response.Current_status.Should().Be(ConferenceState.NotStarted);
            response.Hearing_id.Should().Be(hearing.Id);
            response.Hearing_venue_name.Should().Be(hearing.Hearing_venue_name);
            response.Id.Should().NotBeEmpty();
            response.Scheduled_date_time.Should().Be(hearing.Scheduled_date_time);
            response.Scheduled_duration.Should().Be(hearing.Scheduled_duration);
            response.Started_date_time.Should().BeNull();
            VerifyConferenceParticipants(response.Participants, hearing.Participants);
        }

        private static void VerifyConferenceParticipants(IReadOnlyCollection<ParticipantDetailsResponse> hearingParticipants,
            IReadOnlyCollection<ParticipantResponse> conferenceParticipants)
        {
            hearingParticipants.Count.Should().Be(conferenceParticipants.Count);
            foreach (var hearingParticipant in hearingParticipants)
            {
                var conferenceParticipant =
                    conferenceParticipants.First(x => x.Last_name.Equals(hearingParticipant.Last_name));

                conferenceParticipant.AdditionalProperties.Should().BeEmpty();
                conferenceParticipant.Case_role_name.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Contact_email.Should().Be(hearingParticipant.Contact_email);
                conferenceParticipant.Display_name.Should().Be(hearingParticipant.Display_name);
                conferenceParticipant.First_name.Should().Be(hearingParticipant.First_name);
                conferenceParticipant.Hearing_role_name.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Id.Should().NotBeEmpty();
                conferenceParticipant.Middle_names.Should().BeNullOrWhiteSpace();
                conferenceParticipant.Last_name.Should().Be(hearingParticipant.Last_name);
                conferenceParticipant.Telephone_number.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Title.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Username.Should().Be(hearingParticipant.Username);

                if (!conferenceParticipant.User_role_name.Equals("Representative")) continue;
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
                .Excluding(x => x.AdditionalProperties)
                .Excluding(x => x.Cases)
                .Excluding(x => x.Hearing_room_name)
                .Excluding(x => x.Other_information)
                .Excluding(x => x.Questionnaire_not_required)
                .Excluding(x => x.Updated_by)
            );

            conferenceDetails.Case_name.Should().Be(request.Cases.First().Name);
            conferenceDetails.Case_number.Should().Be(request.Cases.First().Number);
        }

        public static void ParticipantDetails(ParticipantDetailsResponse participant, AddParticipantsToHearingRequest request)
        {
            participant.Should().BeEquivalentTo(request.Participants.First(),
                options => options.ExcludingMissingMembers().Excluding(x => x.Representee));
        }
    }
}
