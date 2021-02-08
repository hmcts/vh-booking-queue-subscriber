using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.Api.Uris;
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
            var uri = BookingsApiUriFactory.HearingsParticipantsEndpoints.AddParticipantsToHearing(Hearing.Id);

            var request = new AddParticipantsToHearingRequest()
            {
                Participants = new HearingParticipantsBuilder(Context.Config.UsernameStem, false).AddUser("Individual", 2).Build()
            };

            await SendPostRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.NoContent, true);

            var conferenceDetails = await PollForConferenceParticipantPresence(Hearing.Id, request.Participants.First().Username, true);
            var participant = conferenceDetails.Participants.First(x => x.Username == request.Participants.First().Username);
            Verify.ParticipantDetails(participant, request);
        }

        [Test]
        public async Task Should_remove_participant_from_hearing_and_conference()
        {
            var participant = Hearing.Participants.First(x => x.UserRoleName.Equals("Individual"));
            var uri = BookingsApiUriFactory.HearingsParticipantsEndpoints.RemoveParticipantFromHearing(Hearing.Id, participant.Id);

            await SendDeleteRequest(uri);
            VerifyResponse(HttpStatusCode.NoContent, true);

            var conferenceDetails = await PollForConferenceParticipantPresence(Hearing.Id, participant.Username, false);
            conferenceDetails.Participants.Any(x => x.Username.Equals(participant.Username)).Should().BeFalse();
        }

        private async Task<ConferenceDetailsResponse> PollForConferenceParticipantPresence(Guid hearingRefId, string username, bool expected)
        {
            var uri = $"{Context.Config.Services.VideoApiUrl}{VideoApiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId)}";
            CreateNewVideoApiClient();

            var policy = Policy
                .HandleResult<HttpResponseMessage>(r => r.Content.ReadAsStringAsync().Result.Contains(username).Equals(!expected))
                .WaitAndRetryAsync(RETRIES, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var result = await policy.ExecuteAsync(async () => await Client.GetAsync(uri));
                var conferenceResponse = RequestHelper.Deserialise<ConferenceDetailsResponse>(await result.Content.ReadAsStringAsync());
                conferenceResponse.CaseName.Should().NotBeNullOrWhiteSpace();
                return conferenceResponse;
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {Math.Pow(2, RETRIES +1)} seconds.");
            }
        }

        [Test]
        public async Task Should_update_participant_in_hearing_and_conference()
        {
            var participant = Hearing.Participants.First(x => x.UserRoleName.Equals("Representative"));
            var uri = BookingsApiUriFactory.HearingsParticipantsEndpoints.UpdateParticipantDetails(Hearing.Id, participant.Id);

            var request = new UpdateParticipantRequest
            {
                DisplayName = $"{participant.DisplayName} {HearingData.UPDATED_TEXT}",
                OrganisationName = $"{participant.Organisation} {HearingData.UPDATED_TEXT}",
                Representee = $"{participant.Representee} {HearingData.UPDATED_TEXT}",
                TelephoneNumber = UserData.UPDATED_TELEPHONE_NUMBER,
                Title = $"{participant.Title} {HearingData.UPDATED_TEXT}"
            };

            await SendPutRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.OK, true);

            var conferenceDetails = await PollForConferenceParticipantUpdated(Hearing.Id, HearingData.UPDATED_TEXT);
            var updatedParticipant = conferenceDetails.Participants.First(x => x.Username.Equals(participant.Username));
            updatedParticipant.DisplayName.Should().Be(request.DisplayName);
            updatedParticipant.Representee.Should().Be(request.Representee);
            updatedParticipant.ContactTelephone.Should().Be(request.TelephoneNumber);
        }

        private async Task<ConferenceDetailsResponse> PollForConferenceParticipantUpdated(Guid hearingRefId, string updatedText)
        {
            var uri = $"{Context.Config.Services.VideoApiUrl}{VideoApiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId)}";
            CreateNewVideoApiClient();

            var policy = Policy
                .HandleResult<HttpResponseMessage>(r => r.Content.ReadAsStringAsync().Result.Contains(updatedText).Equals(false))
                .WaitAndRetryAsync(RETRIES, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var result = await policy.ExecuteAsync(async () => await Client.GetAsync(uri));
                var conferenceResponse = RequestHelper.Deserialise<ConferenceDetailsResponse>(await result.Content.ReadAsStringAsync());
                conferenceResponse.CaseName.Should().NotBeNullOrWhiteSpace();
                return conferenceResponse;
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {Math.Pow(2, RETRIES +1)} seconds.");
            }
        }
    }
}
