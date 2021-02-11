using System;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Builders;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingsApi.Contract.Requests;
using FluentAssertions;
using NUnit.Framework;
using Polly;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.AcceptanceTests.Tests
{
    public class ParticipantSubscriberTests : TestsBase
    {
        [Test]
        public async Task Should_add_participant_to_hearing_and_conference()
        {
            await CreateAndConfirmHearing();

            var request = new AddParticipantsToHearingRequest()
            {
                Participants = new HearingParticipantsBuilder(Context.Config.UsernameStem, false).AddUser("Individual", 2).Build()
            };
            await BookingApiClient.AddParticipantsToHearingAsync(Hearing.Id, request);

            var conferenceDetails = await PollForConferenceParticipantPresence(Hearing.Id, request.Participants.First().Username, true);
            var participant = conferenceDetails.Participants.First(x => x.Username == request.Participants.First().Username);
            Verify.ParticipantDetails(participant, request);
        }

        [Test]
        public async Task Should_remove_participant_from_hearing_and_conference()
        {
            await CreateAndConfirmHearing();

            var participant = Hearing.Participants.First(x => x.UserRoleName.Equals("Individual"));
            
            await BookingApiClient.RemoveParticipantFromHearingAsync(Hearing.Id, participant.Id);
            
            var conferenceDetails = await PollForConferenceParticipantPresence(Hearing.Id, participant.Username, false);
            conferenceDetails.Participants.Any(x => x.Username.Equals(participant.Username)).Should().BeFalse();
        }

        [Test]
        public async Task Should_update_participant_in_hearing_and_conference()
        {
            await CreateAndConfirmHearing();

            var participant = Hearing.Participants.First(x => x.UserRoleName.Equals("Representative"));
            
            var request = new UpdateParticipantRequest
            {
                DisplayName = $"{participant.DisplayName} {HearingData.UPDATED_TEXT}",
                OrganisationName = $"{participant.Organisation} {HearingData.UPDATED_TEXT}",
                Representee = $"{participant.Representee} {HearingData.UPDATED_TEXT}",
                TelephoneNumber = UserData.UPDATED_TELEPHONE_NUMBER,
                Title = $"{participant.Title} {HearingData.UPDATED_TEXT}"
            };

            await BookingApiClient.UpdateParticipantDetailsAsync(Hearing.Id, participant.Id, request);

            var conferenceDetails = await PollForConferenceParticipantUpdated(Hearing.Id, participant.Id,HearingData.UPDATED_TEXT);
            var updatedParticipant = conferenceDetails.Participants.First(x => x.Username.Equals(participant.Username));
            updatedParticipant.DisplayName.Should().Be(request.DisplayName);
            updatedParticipant.Representee.Should().Be(request.Representee);
            updatedParticipant.ContactTelephone.Should().Be(request.TelephoneNumber);
        }
        
        private async Task<ConferenceDetailsResponse> PollForConferenceParticipantPresence(Guid hearingRefId, string username, bool expected)
        {
            var policy = Policy
                .HandleResult<ConferenceDetailsResponse>(conf => conf.Participants.Any(p => p.Username == username) != expected)
                .WaitAndRetryAsync(Retries, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var conferenceResponse = await policy.ExecuteAsync(async () =>
                    await VideoApiClient.GetConferenceByHearingRefIdAsync(hearingRefId, false));
                conferenceResponse.CaseName.Should().NotBeNullOrWhiteSpace();
                return conferenceResponse;
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {Math.Pow(2, Retries +1)} seconds.");
            }
        }

        private async Task<ConferenceDetailsResponse> PollForConferenceParticipantUpdated(Guid hearingRefId, Guid participantId, string updatedText)
        {
            var policy = Policy
                .HandleResult<ConferenceDetailsResponse>(conf => !conf.Participants.First(x=> x.RefId == participantId).DisplayName.Contains(updatedText))
                .WaitAndRetryAsync(Retries, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var conferenceResponse = await policy.ExecuteAsync(async () => await VideoApiClient.GetConferenceByHearingRefIdAsync(hearingRefId, false));
                conferenceResponse.CaseName.Should().NotBeNullOrWhiteSpace();
                return conferenceResponse;
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {Math.Pow(2, Retries +1)} seconds.");
            }
        }
    }
}
