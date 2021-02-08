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
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using Polly;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.AcceptanceTests.Tests
{
    public class HearingsSubscriberTests : TestsBase
    {
        [Test]
        public async Task Should_create_cacd_conference_from_hearing()
        {
            var bookingUri = BookingsApiUriFactory.HearingsEndpoints.BookNewHearing;
            var request = new BookHearingRequestBuilder(Context.Config.UsernameStem).CacdHearing().Build();

            await SendPostRequest(bookingUri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.Created, true);

            var bookingsResponse = RequestHelper.Deserialise<HearingDetailsResponse>(Json);
            bookingsResponse.Should().NotBeNull();
            Hearing = bookingsResponse;

            var confirmRequest = new UpdateBookingStatusRequestBuilder()
                .UpdatedBy(HearingData.CREATED_BY(Context.Config.UsernameStem))
                .Build();

            var updateUri = BookingsApiUriFactory.HearingsEndpoints.UpdateHearingStatus(Hearing.Id);
            await SendPatchRequest(updateUri, RequestHelper.Serialise(confirmRequest));

            var response = await GetConferenceByHearingIdPollingAsync(Hearing.Id);
            response.Should().NotBeNull();
            Verify.ConferenceDetailsResponse(response, Hearing);
        }

        [Test]
        public async Task Should_update_conference_when_hearing_updated()
        {
            var updateUri = BookingsApiUriFactory.HearingsEndpoints.UpdateHearingDetails(Hearing.Id);

            var caseRequests = new List<CaseRequest>()
            {
                new CaseRequest()
                {
                    IsLeadCase = Hearing.Cases.First().IsLeadCase,
                    Name = $"{Hearing.Cases.First().Name} {HearingData.UPDATED_TEXT}",
                    Number = $"{Hearing.Cases.First().Number} {HearingData.UPDATED_TEXT}"
                }
            };

            var request = new UpdateHearingRequest()
            {
                AudioRecordingRequired = !Hearing.AudioRecordingRequired,
                Cases = caseRequests,
                HearingRoomName = $"{Hearing.HearingRoomName} {HearingData.UPDATED_TEXT}",
                HearingVenueName = Hearing.HearingVenueName.Equals(HearingData.VENUE_NAME) ? HearingData.VENUE_NAME_ALTERNATIVE : HearingData.VENUE_NAME,
                OtherInformation = $"{Hearing.OtherInformation} {HearingData.UPDATED_TEXT}",
                QuestionnaireNotRequired = !Hearing.QuestionnaireNotRequired,
                ScheduledDateTime = Hearing.ScheduledDateTime.AddMinutes(10),
                ScheduledDuration = Hearing.ScheduledDuration / 2,
                UpdatedBy = EmailData.NON_EXISTENT_USERNAME
            };

            await SendPutRequest(updateUri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.OK, true);

            var hearingDetails = RequestHelper.Deserialise<HearingDetailsResponse>(Json);
            hearingDetails.Should().NotBeNull();
            Verify.UpdatedHearing(hearingDetails, request);

            var response = await GetUpdatedConferenceDetailsPollingAsync(Hearing.Id);
            response.Should().NotBeNull();
            Verify.UpdatedConference(response, request);
        }

        private async Task<ConferenceDetailsResponse> GetUpdatedConferenceDetailsPollingAsync(Guid hearingRefId)
        {
            var uri = $"{Context.Config.Services.VideoApiUrl}{VideoApiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId)}";
            CreateNewVideoApiClient();

            var policy = Policy
                .HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
                .OrResult(message => !message.Content.ReadAsStringAsync().Result.Contains(HearingData.UPDATED_TEXT))
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
        public async Task Should_delete_conference_when_hearing_deleted()
        {
            var deleteUri = BookingsApiUriFactory.HearingsEndpoints.RemoveHearing(Hearing.Id);
            await SendDeleteRequest(deleteUri);
            VerifyResponse(HttpStatusCode.NoContent, true);

            var result = await PollForConferenceDeleted(Hearing.Id);
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task<HttpResponseMessage> PollForConferenceDeleted(Guid hearingRefId)
        {
            var uri = $"{Context.Config.Services.VideoApiUrl}{VideoApiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId)}";
            CreateNewVideoApiClient();

            var policy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.OK)
                .WaitAndRetryAsync(RETRIES, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var result = await policy.ExecuteAsync(async () => await Client.GetAsync(uri));
                return result;
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {Math.Pow(2, RETRIES +1)} seconds.");
            }
        }

        [Test]
        public async Task Should_delete_conference_when_hearing_cancelled()
        {
            var uri = BookingsApiUriFactory.HearingsEndpoints.UpdateHearingStatus(Hearing.Id);
            const string updatedBy = "updated_by_user@email.com";

            var request = new UpdateBookingStatusRequestBuilder()
                .WithStatus(UpdateBookingStatus.Cancelled)
                .UpdatedBy(updatedBy)
                .Build();

            await SendPatchRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.NoContent, true);

            var result = await PollForConferenceDeleted(Hearing.Id);
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}