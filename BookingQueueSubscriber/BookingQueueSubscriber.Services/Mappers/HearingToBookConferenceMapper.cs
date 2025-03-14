using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using ConferenceRole = VideoApi.Contract.Enums.ConferenceRole;
using ConferenceRoomType = VideoApi.Contract.Enums.ConferenceRoomType;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class HearingToBookConferenceMapper
    {
        public static BookNewConferenceRequest MapToBookNewConferenceRequest(HearingDto hearingDto,
            IEnumerable<ParticipantDto> participantDtos,
            IEnumerable<EndpointDto> endpointDtos)
        {
            var participants = participantDtos
                .Select(ParticipantToParticipantRequestMapper.MapToParticipantRequest)
                .ToList();

            var request = new BookNewConferenceRequest
            {
                CaseNumber = hearingDto.CaseNumber,
                CaseName = hearingDto.CaseName,
                CaseType = hearingDto.CaseType,
                ScheduledDuration = hearingDto.ScheduledDuration,
                ScheduledDateTime = hearingDto.ScheduledDateTime,
                HearingRefId = hearingDto.HearingId,
                Participants = participants,
                HearingVenueName = hearingDto.HearingVenueName,
                AudioRecordingRequired = hearingDto.RecordAudio,
                Endpoints = PopulateAddEndpointRequests(endpointDtos, participantDtos).ToList(),
                CaseTypeServiceId = hearingDto.CaseTypeServiceId,
                Supplier = (Supplier)hearingDto.VideoSupplier,
                ConferenceRoomType = (ConferenceRoomType)hearingDto.ConferenceRoomType,
                AudioPlaybackLanguage = hearingDto.IsVenueWelsh ? AudioPlaybackLanguage.EnglishAndWelsh : AudioPlaybackLanguage.English
            };
            
            return request;
        }

        private static List<AddEndpointRequest> PopulateAddEndpointRequests(IEnumerable<EndpointDto> endpointDtos, IEnumerable<ParticipantDto> participantDtos)
        {
            var addEndpointRequests = new List<AddEndpointRequest>();
            foreach (var endpointDto in endpointDtos)
            {
                addEndpointRequests.Add(new AddEndpointRequest
                {
                    DefenceAdvocate = participantDtos.SingleOrDefault(x => x.ContactEmail == endpointDto.DefenceAdvocateContactEmail)?.Username,
                    DisplayName = endpointDto.DisplayName,
                    Pin = endpointDto.Pin,
                    SipAddress = endpointDto.Sip,
                    ConferenceRole = Enum.Parse<ConferenceRole>(endpointDto.Role.ToString())
                });
            }
            return addEndpointRequests;
        }
    }
}