using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Builders;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingQueueSubscriber.AcceptanceTests.Hooks;
using BookingsApi.Client;
using BookingsApi.Contract.Enums;
using BookingsApi.Contract.Responses;
using Castle.Core.Internal;
using FluentAssertions;
using NUnit.Framework;
using Polly;
using VideoApi.Client;
using VideoApi.Contract.Responses;
using TestContext = BookingQueueSubscriber.AcceptanceTests.Hooks.TestContext;

namespace BookingQueueSubscriber.AcceptanceTests.Tests
{
    public class TestsBase
    {
        protected const int Retries = 4; // 4 retries ^2 will execute after 2 seconds, then 4, 8, then finally 16 (30 seconds in total)
        protected TestContext Context;
        protected List<HearingDetailsResponse> Hearings = new List<HearingDetailsResponse>();
        protected HearingDetailsResponse Hearing;
        protected ConferenceDetailsResponse Conference;
        protected BookingsApiClient BookingApiClient;
        protected VideoApiClient VideoApiClient;

        [OneTimeSetUp]
        public void BeforeTestRun()
        {
            Context = new Setup().RegisterSecrets();
            InitApiClient(Context);
        }

        private void InitApiClient(TestContext context)
        {
            NUnit.Framework.TestContext.Out.WriteLine("Initialising API Client");
            var bookingsHttpClient = new HttpClient();
            bookingsHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", context.Tokens.BookingsApiBearerToken);
            BookingApiClient = BookingsApiClient.GetClient(context.Config.Services.BookingsApiUrl, bookingsHttpClient);
            
            var videoHttpClient = new HttpClient();
            videoHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", context.Tokens.VideoApiBearerToken);
            VideoApiClient = VideoApiClient.GetClient(context.Config.Services.VideoApiUrl, videoHttpClient);
        }

        [TearDown]
        public async Task AfterEveryTest()
        {
            if (Hearing != null && (Hearing.Status == BookingStatus.Created || Hearing.Status == BookingStatus.Booked))
            {
                await BookingApiClient.RemoveHearingAsync(Hearing.Id);
            }

            if (!Hearings.IsNullOrEmpty())
            {
                foreach (var hearingId in Hearings.Select(x => x.GroupId).Distinct())
                {
                    if (hearingId!= null)
                    {
                        await BookingApiClient.RemoveHearingAsync((Guid)hearingId);
                    }
                }
            }
        }

        protected async Task CreateAndConfirmHearing()
        {
            await CreateHearing();
            await ConfirmHearing();
        }

        private async Task CreateHearing()
        {
            var request = new BookHearingRequestBuilder(Context.Config.UsernameStem).Build();
            Hearing = await BookingApiClient.BookNewHearingAsync(request);
        }

        protected async Task ConfirmHearing()
        {
            var confirmRequest = new UpdateBookingStatusRequestBuilder()
                .UpdatedBy(HearingData.CREATED_BY(Context.Config.UsernameStem))
                .Build();

            await BookingApiClient.UpdateBookingStatusAsync(Hearing.Id, confirmRequest);

            Conference = await GetConferenceByHearingIdPollingAsync(Hearing.Id);
            Verify.ConferenceDetailsResponse(Conference, Hearing);
        }

        protected async Task<ConferenceDetailsResponse> GetConferenceByHearingIdPollingAsync(Guid hearingRefId)
        {
            var policy = Policy
                .Handle<VideoApiException>(e => e.StatusCode == (int) HttpStatusCode.NotFound)
                .OrResult<ConferenceDetailsResponse>(message => message == null)
                .WaitAndRetryAsync(Retries, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var conferenceResponse = await policy.ExecuteAsync(async () => await VideoApiClient.GetConferenceByHearingRefIdAsync(hearingRefId, false));
                conferenceResponse.Should().NotBeNull();
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
