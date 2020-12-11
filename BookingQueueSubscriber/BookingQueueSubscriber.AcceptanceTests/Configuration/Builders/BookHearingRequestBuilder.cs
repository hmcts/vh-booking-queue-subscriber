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
        private readonly string _usernameStem;

        public BookHearingRequestBuilder(string usernameStem)
        {
            _randomNumber = new Random();
            _usernameStem = usernameStem;
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
                Scheduled_duration = HearingData.SCHEDULED_DURATION
            };
        }

        public BookHearingRequestBuilder CacdHearing()
        {
            _request.Case_type_name = HearingData.CACD_CASE_TYPE_NAME;
            _request.Hearing_type_name = HearingData.CACD_HEARING_TYPE_NAME;
            _request.Participants = new HearingParticipantsBuilder(_usernameStem, true)
                .AddJudge()
                .AddIndividual()
                .AddRep()
                .AddWinger()
                .Build();
            return this;
        }

        public void AddParticipants()
        {
            _request.Participants ??= new HearingParticipantsBuilder(_usernameStem, false)
                .AddJudge()
                .AddIndividual()
                .AddRep()
                .AddObserver()
                .AddPanelMember()
                .AddJudicialOfficeHolder()
                .Build();
        }

        public BookNewHearingRequest Build()
        {
            AddParticipants();

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
