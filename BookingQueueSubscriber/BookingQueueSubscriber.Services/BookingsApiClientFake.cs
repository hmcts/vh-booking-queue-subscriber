using BookingsApi.Client;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookingQueueSubscriber.Services
{
    public class BookingsApiClientFake : IBookingsApiClient
    {
        public Task<BookingsApi.Contract.Responses.EndpointResponse> AddEndPointToHearingAsync(Guid hearingId, BookingsApi.Contract.Requests.AddEndpointRequest addEndpointRequest)
        {
            throw new NotImplementedException();
        }

        public Task<BookingsApi.Contract.Responses.EndpointResponse> AddEndPointToHearingAsync(Guid hearingId, BookingsApi.Contract.Requests.AddEndpointRequest addEndpointRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddParticipantsToHearingAsync(Guid hearingId, AddParticipantsToHearingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task AddParticipantsToHearingAsync(Guid hearingId, AddParticipantsToHearingRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AnonymiseHearingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task AnonymiseHearingsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AnonymiseParticipantAndCaseByHearingIdAsync(string hearingIdsPath, IEnumerable<Guid> hearingIdsBody)
        {
            throw new NotImplementedException();
        }

        public Task AnonymiseParticipantAndCaseByHearingIdAsync(string hearingIdsPath, IEnumerable<Guid> hearingIdsBody, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AnonymisePersonWithUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task AnonymisePersonWithUsernameAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AnonymisePersonWithUsernameForExpiredHearingsAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task AnonymisePersonWithUsernameForExpiredHearingsAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponse> BookNewHearingAsync(BookNewHearingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponse> BookNewHearingAsync(BookNewHearingRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BulkJudiciaryLeaverResponse> BulkJudiciaryLeaversAsync(IEnumerable<JudiciaryLeaverRequest> request)
        {
            throw new NotImplementedException();
        }

        public Task<BulkJudiciaryLeaverResponse> BulkJudiciaryLeaversAsync(IEnumerable<JudiciaryLeaverRequest> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BulkJudiciaryPersonResponse> BulkJudiciaryPersonsAsync(IEnumerable<JudiciaryPersonRequest> request)
        {
            throw new NotImplementedException();
        }

        public Task<BulkJudiciaryPersonResponse> BulkJudiciaryPersonsAsync(IEnumerable<JudiciaryPersonRequest> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task BulkJudiciaryPersonsStagingAsync(IEnumerable<JudiciaryPersonStagingRequest> request)
        {
            throw new NotImplementedException();
        }

        public Task BulkJudiciaryPersonsStagingAsync(IEnumerable<JudiciaryPersonStagingRequest> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BookingsApiHealthResponse> CheckServiceHealth2Async()
        {
            throw new NotImplementedException();
        }

        public Task<BookingsApiHealthResponse> CheckServiceHealth2Async(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BookingsApiHealthResponse> CheckServiceHealthAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BookingsApiHealthResponse> CheckServiceHealthAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CloneHearingAsync(Guid hearingId, CloneHearingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task CloneHearingAsync(Guid hearingId, CloneHearingRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ParticipantResponse>> GetAllParticipantsInHearingAsync(Guid hearingId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ParticipantResponse>> GetAllParticipantsInHearingAsync(Guid hearingId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<AnonymisationDataResponse> GetAnonymisationDataAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AnonymisationDataResponse> GetAnonymisationDataAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CaseRoleResponse>> GetCaseRolesForCaseTypeAsync(string caseTypeName)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CaseRoleResponse>> GetCaseRolesForCaseTypeAsync(string caseTypeName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CaseTypeResponse>> GetCaseTypesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CaseTypeResponse>> GetCaseTypesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetFeatureFlagAsync(string featureName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetFeatureFlagAsync(string featureName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponse> GetHearingDetailsByIdAsync(Guid hearingId)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponse> GetHearingDetailsByIdAsync(Guid hearingId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingRoleResponse>> GetHearingRolesForCaseRoleAsync(string caseTypeName, string caseRoleName)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingRoleResponse>> GetHearingRolesForCaseRoleAsync(string caseTypeName, string caseRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponse>> GetHearingsByGroupIdAsync(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponse>> GetHearingsByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BookingsResponse> GetHearingsByTypesAsync(GetHearingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BookingsResponse> GetHearingsByTypesAsync(GetHearingRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponse>> GetHearingsByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponse>> GetHearingsByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingsByUsernameForDeletionResponse>> GetHearingsByUsernameForDeletionAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingsByUsernameForDeletionResponse>> GetHearingsByUsernameForDeletionAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponse>> GetHearingsForNotificationAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponse>> GetHearingsForNotificationAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingVenueResponse>> GetHearingVenuesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingVenueResponse>> GetHearingVenuesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipantResponse> GetParticipantInHearingAsync(Guid hearingId, Guid participantId)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipantResponse> GetParticipantInHearingAsync(Guid hearingId, Guid participantId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ParticipantResponse>> GetParticipantsByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ParticipantResponse>> GetParticipantsByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserWithClosedConferencesResponse> GetPersonByClosedHearingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserWithClosedConferencesResponse> GetPersonByClosedHearingsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PersonResponse> GetPersonByContactEmailAsync(string contactEmail)
        {
            throw new NotImplementedException();
        }

        public Task<PersonResponse> GetPersonByContactEmailAsync(string contactEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PersonResponse> GetPersonByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<PersonResponse> GetPersonByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PersonSuitabilityAnswerResponse>> GetPersonSuitabilityAnswersAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PersonSuitabilityAnswerResponse>> GetPersonSuitabilityAnswersAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PersonResponse>> GetStaffMemberBySearchTermAsync(string term)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PersonResponse>> GetStaffMemberBySearchTermAsync(string term, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<SuitabilityAnswersResponse> GetSuitabilityAnswersAsync(string cursor, int? limit)
        {
            throw new NotImplementedException();
        }

        public Task<SuitabilityAnswersResponse> GetSuitabilityAnswersAsync(string cursor, int? limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PersonResponse>> PostJudiciaryPersonBySearchTermAsync(SearchTermRequest term)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PersonResponse>> PostJudiciaryPersonBySearchTermAsync(SearchTermRequest term, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PersonResponse>> PostPersonBySearchTermAsync(SearchTermRequest term)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PersonResponse>> PostPersonBySearchTermAsync(SearchTermRequest term, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllJudiciaryPersonsStagingAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllJudiciaryPersonsStagingAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveEndPointFromHearingAsync(Guid hearingId, Guid endpointId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveEndPointFromHearingAsync(Guid hearingId, Guid endpointId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveHearingAsync(Guid hearingId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveHearingAsync(Guid hearingId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveParticipantFromHearingAsync(Guid hearingId, Guid participantId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveParticipantFromHearingAsync(Guid hearingId, Guid participantId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<AudioRecordedHearingsBySearchResponse>> SearchForHearingsAsync(string caseNumber, DateTimeOffset? date)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<AudioRecordedHearingsBySearchResponse>> SearchForHearingsAsync(string caseNumber, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PersonResponse> SearchForNonJudgePersonsByContactEmailAsync(string contactEmail)
        {
            throw new NotImplementedException();
        }

        public Task<PersonResponse> SearchForNonJudgePersonsByContactEmailAsync(string contactEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBookingStatusAsync(Guid hearingId, UpdateBookingStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBookingStatusAsync(Guid hearingId, UpdateBookingStatusRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDisplayNameForEndpointAsync(Guid hearingId, Guid endpointId, BookingsApi.Contract.Requests.UpdateEndpointRequest updateEndpointRequest)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDisplayNameForEndpointAsync(Guid hearingId, Guid endpointId, BookingsApi.Contract.Requests.UpdateEndpointRequest updateEndpointRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponse> UpdateHearingDetailsAsync(Guid hearingId, UpdateHearingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponse> UpdateHearingDetailsAsync(Guid hearingId, UpdateHearingRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateHearingParticipantsAsync(Guid hearingId, UpdateHearingParticipantsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task UpdateHearingParticipantsAsync(Guid hearingId, UpdateHearingParticipantsRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateParticipantDetailsAsync(Guid hearingId, Guid participantId, BookingsApi.Contract.Requests.UpdateParticipantRequest request)
        {
            throw new NotImplementedException();
        }

        public Task UpdateParticipantDetailsAsync(Guid hearingId, Guid participantId, BookingsApi.Contract.Requests.UpdateParticipantRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePersonDetailsAsync(Guid personId, UpdatePersonDetailsRequest payload)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePersonDetailsAsync(Guid personId, UpdatePersonDetailsRequest payload, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePersonUsernameAsync(string contactEmail, string username)
        {
           return Task.CompletedTask;
        }

        public Task UpdatePersonUsernameAsync(string contactEmail, string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateSuitabilityAnswersAsync(Guid hearingId, Guid participantId, IEnumerable<SuitabilityAnswersRequest> answers)
        {
            throw new NotImplementedException();
        }

        public Task UpdateSuitabilityAnswersAsync(Guid hearingId, Guid participantId, IEnumerable<SuitabilityAnswersRequest> answers, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}