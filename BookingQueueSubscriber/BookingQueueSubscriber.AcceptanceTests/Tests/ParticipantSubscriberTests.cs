using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.Api.Uris;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Builders;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingQueueSubscriber.Services.BookingsApi;
using BookingQueueSubscriber.Services.VideoApi;
using FluentAssertions;
using NUnit.Framework;
using Polly;
using UpdateParticipantRequest = BookingQueueSubscriber.Services.BookingsApi.UpdateParticipantRequest;

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
            var participant = Hearing.Participants.First(x => x.User_role_name.Equals("Individual"));
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
                conferenceResponse.Case_name.Should().NotBeNullOrWhiteSpace();
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
            // var participant = Hearing.Participants.First(x => x.User_role_name.Equals("Representative"));
            var hearingId = Guid.Parse("68e46a4f-1b4f-4227-b456-a11acd6292cd");
            var participantId = Guid.Parse("62c8dec2-c601-4c8a-9d10-b2e7a2b141e8");
            var uri = BookingsApiUriFactory.HearingsParticipantsEndpoints.UpdateParticipantDetails(hearingId, participantId);

            var request = new UpdateParticipantRequest
            {
                Display_name = $"Dev Test Observer 2 {HearingData.UPDATED_TEXT}",
                Telephone_number = UserData.UPDATED_TELEPHONE_NUMBER,
                Title = $"{HearingData.UPDATED_TEXT}"
            };

            await SendPutRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.OK, true);

            var conferenceDetails = await PollForConferenceParticipantUpdated(Hearing.Id, HearingData.UPDATED_TEXT);
            var updatedParticipant = conferenceDetails.Participants.First(x => x.Username.Equals("dev345.test345.observer345@hearings.reform.hmcts.net"));
            updatedParticipant.Display_name.Should().Be(request.Display_name);
            updatedParticipant.Representee.Should().Be(request.Representee);
            updatedParticipant.Contact_telephone.Should().Be(request.Telephone_number);
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
                conferenceResponse.Case_name.Should().NotBeNullOrWhiteSpace();
                return conferenceResponse;
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {Math.Pow(2, RETRIES +1)} seconds.");
            }
        }
    }
}
