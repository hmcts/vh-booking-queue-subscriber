using System;
using System.Collections.Generic;
using System.Linq;
using AcceptanceTests.Common.Data.Helpers;
using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingsApi.Contract.Requests;

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
                AudioRecordingRequired = HearingData.AUDIO_RECORDING_REQUIRED,
                CaseTypeName = HearingData.CASE_TYPE_NAME,
                CreatedBy = EmailData.NON_EXISTENT_USERNAME,
                Endpoints = new List<EndpointRequest>(),
                HearingRoomName = HearingData.HEARING_ROOM_NAME,
                HearingTypeName = HearingData.HEARING_TYPE_NAME,
                HearingVenueName = HearingData.VENUE_NAME,
                OtherInformation = HearingData.OTHER_INFORMATION,
                QuestionnaireNotRequired = HearingData.QUESTIONNAIRE_NOT_REQUIRED,
                ScheduledDateTime = DateTime.UtcNow.AddMinutes(5),
                ScheduledDuration = HearingData.SCHEDULED_DURATION
            };
        }

        public BookHearingRequestBuilder CacdHearing()
        {
            _request.CaseTypeName = HearingData.CACD_CASE_TYPE_NAME;
            _request.HearingTypeName = HearingData.CACD_HEARING_TYPE_NAME;
            _request.Participants = new HearingParticipantsBuilder(_usernameStem, true)
                .AddJudge()
                .AddIndividual()
                .AddRep()
                .AddWinger()
                .Build();
            return this;
        }

        public BookHearingRequestBuilder AddParticipants()
        {
            if (!_request.Participants.Any())
            {
                _request.Participants = new HearingParticipantsBuilder(_usernameStem, false)
                    .AddJudge()
                    .AddIndividual()
                    .AddRep()
                    .AddObserver()
                    .AddPanelMember()
                    .Build();
            }
            return this;
        }

        public BookNewHearingRequest Build()
        {
            AddParticipants();

            _request.Cases = new List<CaseRequest>()
            {
                new CaseRequest()
                {
                    IsLeadCase = HearingData.IS_LEAD_CASE,
                    Name = $"BQS {HearingData.AUTOMATED_CASE_NAME_PREFIX} {GenerateRandom.Letters(_randomNumber)}",
                    Number = GenerateRandom.CaseNumber(_randomNumber)
                }
            };
            return _request;
        }
    }
}
