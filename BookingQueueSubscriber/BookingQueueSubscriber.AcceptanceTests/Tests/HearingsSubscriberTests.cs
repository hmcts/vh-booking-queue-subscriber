using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Builders;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingQueueSubscriber.AcceptanceTests.Helpers;
using BookingsApi.Contract.Enums;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using Polly;
using VideoApi.Client;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.AcceptanceTests.Tests
{
    public class HearingsSubscriberTests : TestsBase
    {
        [Test]
        public async Task Should_create_single_day_conference_from_hearing()
        {
            await CreateAndConfirmHearing();
            Verify.ConferenceDetailsResponse(Conference, Hearing);
        }

        [Test]
        public async Task Should_create_multi_day_conference_from_hearing()
        {
            var request = new BookHearingRequestBuilder(Context.Config.UsernameStem).MoveScheduledDateIfWeekend().Build();
            var initialHearing = await BookingApiClient.BookNewHearingAsync(request);
            Hearings.Add(initialHearing);

            const int numberOfDays = 2;
            var numberOfDaysNotIncludingWeekends = CalculateDaysWithoutWeekend(numberOfDays, initialHearing.ScheduledDateTime);
            var start = initialHearing.ScheduledDateTime;
            var end = initialHearing.ScheduledDateTime.AddDays(numberOfDaysNotIncludingWeekends);
            var dates = DateListMapper.GetListOfWorkingDates(start, end);
            dates.Count.Should().Be(numberOfDays);
            // Add the initial hearing day
            const int totalNumberOfDays = numberOfDays + 1;

            var multiHearingsRequest = new CloneHearingRequest()
            {
                Dates = dates
            };

            await BookingApiClient.CloneHearingAsync(initialHearing.Id, multiHearingsRequest);

            var anyParticipant = initialHearing.Participants.First();

            var multiDays = await PollForAllMultiDayHearings(anyParticipant.Username, initialHearing.Cases.Single().Name, totalNumberOfDays);
            Hearings.AddRange(multiDays);
            multiDays.Count.Should().Be(totalNumberOfDays);

            foreach (var day in multiDays)
            {
                await ConfirmMultiDayHearing(day);
            }
        }

        private static int CalculateDaysWithoutWeekend(int days, DateTime startDateTime)
        {
            if (startDateTime.AddDays(2).DayOfWeek == DayOfWeek.Saturday ||
                startDateTime.AddDays(2).DayOfWeek == DayOfWeek.Sunday)
            {
                days += 2;
            }

            return days;
        }

        private async Task<List<HearingDetailsResponse>> PollForAllMultiDayHearings(string username, string expectedCaseName, int expectedNumberOfHearings)
        {
            var policy = Policy
                .HandleResult<List<HearingDetailsResponse>>(responses => responses.Where(x => x.Cases.Single().Name.StartsWith(expectedCaseName)).ToList().Count != expectedNumberOfHearings)
                .WaitAndRetryAsync(Retries, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var allHearingsForUser = await policy.ExecuteAsync(async () => await BookingApiClient.GetHearingsByUsernameAsync(username) as List<HearingDetailsResponse>);
            var multiDayHearingsOnly = allHearingsForUser.Where(x => x.Cases.Single().Name.StartsWith(expectedCaseName)).ToList();
            multiDayHearingsOnly.Count.Should().Be(expectedNumberOfHearings);
            return multiDayHearingsOnly;
        }

        private async Task ConfirmMultiDayHearing(HearingDetailsResponse hearing)
        {
            var confirmRequest = new UpdateBookingStatusRequestBuilder()
                .UpdatedBy(HearingData.CREATED_BY(Context.Config.UsernameStem))
                .Build();

            await BookingApiClient.UpdateBookingStatusAsync(hearing.Id, confirmRequest);

            Conference = await GetConferenceByHearingIdPollingAsync(hearing.Id);
            Verify.ConferenceDetailsResponse(Conference, hearing);
        }

        [Test]
        public async Task Should_create_cacd_conference_from_hearing()
        {
            await CreateAndConfirmHearing();

            var request = new BookHearingRequestBuilder(Context.Config.UsernameStem).CacdHearing().Build();

            var hearing = await BookingApiClient.BookNewHearingAsync(request);

            var confirmRequest = new UpdateBookingStatusRequestBuilder()
                .UpdatedBy(HearingData.CREATED_BY(Context.Config.UsernameStem))
                .Build();

            await BookingApiClient.UpdateBookingStatusAsync(hearing.Id, confirmRequest);
            var conferenceDetailsResponse = await GetConferenceByHearingIdPollingAsync(Hearing.Id);
            conferenceDetailsResponse.Should().NotBeNull();
            Verify.ConferenceDetailsResponse(conferenceDetailsResponse, Hearing);
        }

        [Test]
        public async Task Should_update_conference_when_hearing_updated()
        {
            await CreateAndConfirmHearing();

            var caseRequests = new List<CaseRequest>
            {
                new CaseRequest
                {
                    IsLeadCase = Hearing.Cases.First().IsLeadCase,
                    Name = $"{Hearing.Cases.First().Name} {HearingData.UPDATED_TEXT}",
                    Number = $"{Hearing.Cases.First().Number} {HearingData.UPDATED_TEXT}"
                }
            };

            var request = new UpdateHearingRequest
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

            var hearingDetails = await BookingApiClient.UpdateHearingDetailsAsync(Hearing.Id, request);
            Verify.UpdatedHearing(hearingDetails, request);

            var response = await GetUpdatedConferenceDetailsPollingAsync(Hearing.Id);
            response.Should().NotBeNull();
            Verify.UpdatedConference(response, request);
        }

        private async Task<ConferenceDetailsResponse> GetUpdatedConferenceDetailsPollingAsync(Guid hearingRefId)
        {
            var policy = Policy
                .HandleResult<ConferenceDetailsResponse>(conf => conf != null)
                .OrResult(conf => !conf.HearingVenueName.Contains(HearingData.UPDATED_TEXT))
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

        [Test]
        public async Task Should_delete_conference_when_hearing_deleted()
        {
            await CreateAndConfirmHearing();
            await BookingApiClient.RemoveHearingAsync(Hearing.Id);
            var result = await PollForConferenceDeleted(Hearing.Id);
            Hearing.Status = BookingStatus.Cancelled;
            result.Should().BeTrue();
        }

        [Test]
        public async Task Should_delete_conference_when_hearing_cancelled()
        {
            await CreateAndConfirmHearing();

            const string updatedBy = "updated_by_user@email.com";

            var request = new UpdateBookingStatusRequestBuilder()
                .WithStatus(UpdateBookingStatus.Cancelled)
                .UpdatedBy(updatedBy)
                .Build();
            await BookingApiClient.UpdateBookingStatusAsync(Hearing.Id, request);
            
            var result = await PollForConferenceDeleted(Hearing.Id);
            Hearing.Status = BookingStatus.Cancelled;
            result.Should().BeTrue();
        }
        
        private async Task<bool> PollForConferenceDeleted(Guid hearingRefId)
        {
            var policy = Policy
                .HandleResult<ConferenceDetailsResponse>(e => e != null)
                .WaitAndRetryAsync(Retries, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var conferenceResponse = await policy.ExecuteAsync(async () =>
                    await VideoApiClient.GetConferenceByHearingRefIdAsync(hearingRefId, false));
                if (conferenceResponse != null)
                {
                    return false;
                }
            }
            catch (VideoApiException e)
            {
                if (e.StatusCode == (int) HttpStatusCode.NotFound)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {Math.Pow(2, Retries +1)} seconds.");
            }

            return true;
        }
    }
}