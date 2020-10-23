using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AcceptanceTests.Common.Data.Helpers;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingQueueSubscriber.Services.BookingsApi;

namespace BookingQueueSubscriber.AcceptanceTests.Configuration.Builders
{
    public class BookHearingRequestBuilder
    {
        private readonly BookNewHearingRequest _request;
        private readonly Random _randomNumber;

        public BookHearingRequestBuilder(string usernameStem)
        {
            _randomNumber = new Random();

            _request = new BookNewHearingRequest
            {
                AdditionalProperties = new ConcurrentDictionary<string, object>(),
                Audio_recording_required = HearingData.AUDIO_RECORDING_REQUIRED,
                Case_type_name = HearingData.CASE_TYPE_NAME,
                Created_by = EmailData.NON_EXISTENT_USERNAME,
                Endpoints = new List<EndpointRequest>(),
                Hearing_room_name = HearingData.HEARING_ROOM_NAME,
                Hearing_type_name = HearingData.HEARING_TYPE_NAME,
                Hearing_venue_name = HearingData.VENUE_NAME,
                Other_information = HearingData.OTHER_INFORMATION,
                Questionnaire_not_required = HearingData.QUESTIONNAIRE_NOT_REQUIRED,
                Scheduled_date_time = DateTime.UtcNow.AddMinutes(5),
                Scheduled_duration = HearingData.SCHEDULED_DURATION,
                Participants = new HearingParticipantsBuilder(usernameStem)
                    .AddJudge()
                    .AddIndividual()
                    .AddRep()
                    .AddObserver()
                    .AddPanelMember()
                    .Build()
            };
        }

        public BookNewHearingRequest Build()
        {
            _request.Cases = new List<CaseRequest>()
            {
                new CaseRequest()
                {
                    AdditionalProperties = new ConcurrentDictionary<string, object>(),
                    Is_lead_case = HearingData.IS_LEAD_CASE,
                    Name = $"BQS {HearingData.AUTOMATED_CASE_NAME_PREFIX} {GenerateRandom.Letters(_randomNumber)}",
                    Number = GenerateRandom.CaseNumber(_randomNumber)
        }
            };
            return _request;
        }
    }
}
