using BookingsApi.Client;
using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using System.Diagnostics.CodeAnalysis;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;

namespace BookingQueueSubscriber.Services
{
    [ExcludeFromCodeCoverage]
    public class BookingsApiClientFake : IBookingsApiClient
    {
        public Task RemoveEndPointFromHearingAsync(Guid hearingId, Guid endpointId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveEndPointFromHearingAsync(Guid hearingId, Guid endpointId, CancellationToken cancellationToken)
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

        public Task AnonymiseParticipantAndCaseByHearingIdAsync(IEnumerable<Guid> hearingIds)
        {
            throw new NotImplementedException();
        }

        public Task AnonymiseParticipantAndCaseByHearingIdAsync(IEnumerable<Guid> hearingIds, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CancelHearingsInGroupAsync(Guid groupId, CancelHearingsInGroupRequest request)
        {
            throw new NotImplementedException();
        }

        public Task CancelHearingsInGroupAsync(Guid groupId, CancelHearingsInGroupRequest request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RebookHearingAsync(Guid hearingId)
        {
            throw new NotImplementedException();
        }

        public Task RebookHearingAsync(Guid hearingId, CancellationToken cancellationToken)
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

        public Task UpdateBookingStatusAsync(Guid hearingId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBookingStatusAsync(Guid hearingId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task FailBookingAsync(Guid hearingId)
        {
            throw new NotImplementedException();
        }

        public Task FailBookingAsync(Guid hearingId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CancelBookingAsync(Guid hearingId, CancelBookingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task CancelBookingAsync(Guid hearingId, CancelBookingRequest request, CancellationToken cancellationToken)
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

        public Task<ICollection<AudioRecordedHearingsBySearchResponse>> SearchForHearingsAsync(string caseNumber, DateTimeOffset? date)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<AudioRecordedHearingsBySearchResponse>> SearchForHearingsAsync(string caseNumber, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BookingStatus> GetBookingStatusByIdAsync(Guid hearingId)
        {
            throw new NotImplementedException();
        }

        public Task<BookingStatus> GetBookingStatusByIdAsync(Guid hearingId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingVenueResponse>> GetHearingVenuesAsync(bool? excludeExpiredVenue)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingVenueResponse>> GetHearingVenuesAsync(bool? excludeExpiredVenue, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingVenueResponse>> GetHearingVenuesForHearingsTodayAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingVenueResponse>> GetHearingVenuesForHearingsTodayAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<string>> GetHearingVenuesByAllocatedCsoAsync(IEnumerable<Guid> csoIds)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<string>> GetHearingVenuesByAllocatedCsoAsync(IEnumerable<Guid> csoIds, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateJobHistoryAsync(string jobName, bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        public Task UpdateJobHistoryAsync(string jobName, bool isSuccessful, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<JobHistoryResponse>> GetJobHistoryAsync(string jobName)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<JobHistoryResponse>> GetJobHistoryAsync(string jobName, CancellationToken cancellationToken)
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

        public Task<BulkJudiciaryLeaverResponse> BulkJudiciaryLeaversAsync(IEnumerable<JudiciaryLeaverRequest> request)
        {
            throw new NotImplementedException();
        }

        public Task<BulkJudiciaryLeaverResponse> BulkJudiciaryLeaversAsync(IEnumerable<JudiciaryLeaverRequest> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<JudiciaryPersonResponse>> PostJudiciaryPersonBySearchTermAsync(SearchTermRequest term)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<JudiciaryPersonResponse>> PostJudiciaryPersonBySearchTermAsync(SearchTermRequest term, CancellationToken cancellationToken)
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

        public Task BulkJudiciaryPersonsStagingAsync(IEnumerable<JudiciaryPersonStagingRequest> request)
        {
            throw new NotImplementedException();
        }

        public Task BulkJudiciaryPersonsStagingAsync(IEnumerable<JudiciaryPersonStagingRequest> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<JusticeUserResponse> AddJusticeUserAsync(AddJusticeUserRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<JusticeUserResponse> AddJusticeUserAsync(AddJusticeUserRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<JusticeUserResponse> EditJusticeUserAsync(EditJusticeUserRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<JusticeUserResponse> EditJusticeUserAsync(EditJusticeUserRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<JusticeUserResponse> GetJusticeUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<JusticeUserResponse> GetJusticeUserByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<JusticeUserResponse>> GetJusticeUserListAsync(string term, bool? includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<JusticeUserResponse>> GetJusticeUserListAsync(string term, bool? includeDeleted, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteJusticeUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteJusticeUserAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RestoreJusticeUserAsync(RestoreJusticeUserRequest request)
        {
            throw new NotImplementedException();
        }

        public Task RestoreJusticeUserAsync(RestoreJusticeUserRequest request, CancellationToken cancellationToken)
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

        public Task<UserWithClosedConferencesResponse> GetPersonByClosedHearingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserWithClosedConferencesResponse> GetPersonByClosedHearingsAsync(CancellationToken cancellationToken)
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

        public Task UpdatePersonUsernameAsync(string contactEmail, string username)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePersonUsernameAsync(string contactEmail, string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<JusticeUserResponse> AllocateHearingAutomaticallyAsync(Guid hearingId)
        {
            throw new NotImplementedException();
        }

        public Task<JusticeUserResponse> AllocateHearingAutomaticallyAsync(Guid hearingId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<AllocatedCsoResponse>> GetAllocationsForHearingsByVenueAsync(IEnumerable<string> hearingVenueNames)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<AllocatedCsoResponse>> GetAllocationsForHearingsByVenueAsync(IEnumerable<string> hearingVenueNames, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingAllocationsResponse>> SearchForAllocationHearingsAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate, IEnumerable<Guid> cso,
            IEnumerable<string> caseType, string caseNumber, bool? isUnallocated)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingAllocationsResponse>> SearchForAllocationHearingsAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate, IEnumerable<Guid> cso,
            IEnumerable<string> caseType, string caseNumber, bool? isUnallocated, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingAllocationsResponse>> AllocateHearingsToCsoAsync(UpdateHearingAllocationToCsoRequest postRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingAllocationsResponse>> AllocateHearingsToCsoAsync(UpdateHearingAllocationToCsoRequest postRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<string>> SaveWorkHoursAsync(IEnumerable<UploadWorkHoursRequest> uploadWorkHoursRequests)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<string>> SaveWorkHoursAsync(IEnumerable<UploadWorkHoursRequest> uploadWorkHoursRequests, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<string>> SaveNonWorkingHoursAsync(IEnumerable<UploadNonWorkingHoursRequest> uploadNonWorkingHoursRequests)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<string>> SaveNonWorkingHoursAsync(IEnumerable<UploadNonWorkingHoursRequest> uploadNonWorkingHoursRequests, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<VhoWorkHoursResponse>> GetVhoWorkAvailabilityHoursAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<VhoWorkHoursResponse>> GetVhoWorkAvailabilityHoursAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<VhoNonAvailabilityWorkHoursResponse>> GetVhoNonAvailabilityHoursAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<VhoNonAvailabilityWorkHoursResponse>> GetVhoNonAvailabilityHoursAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVhoNonAvailabilityHoursAsync(string username, UpdateNonWorkingHoursRequest request)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVhoNonAvailabilityHoursAsync(string username, UpdateNonWorkingHoursRequest request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteVhoNonAvailabilityHoursAsync(string username, long nonAvailabilityId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteVhoNonAvailabilityHoursAsync(string username, long nonAvailabilityId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CaseTypeResponseV2>> GetCaseTypesV2Async(bool? includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CaseTypeResponseV2>> GetCaseTypesV2Async(bool? includeDeleted, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<EndpointResponseV2> AddEndPointToHearingV2Async(Guid hearingId, EndpointRequestV2 addEndpointRequest)
        {
            throw new NotImplementedException();
        }

        public Task<EndpointResponseV2> AddEndPointToHearingV2Async(Guid hearingId, EndpointRequestV2 addEndpointRequest,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateEndpointV2Async(Guid hearingId, Guid endpointId, UpdateEndpointRequestV2 updateEndpointRequest)
        {
            throw new NotImplementedException();
        }

        public Task UpdateEndpointV2Async(Guid hearingId, Guid endpointId, UpdateEndpointRequestV2 updateEndpointRequest,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetHearingsForTodayV2Async()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetHearingsForTodayV2Async(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetHearingsForTodayByVenueV2Async(IEnumerable<string> venueNames)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetHearingsForTodayByVenueV2Async(IEnumerable<string> venueNames, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ConfirmedHearingsTodayResponseV2>> GetConfirmedHearingsByUsernameForTodayV2Async(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ConfirmedHearingsTodayResponseV2>> GetConfirmedHearingsByUsernameForTodayV2Async(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetHearingsForTodayByCsosV2Async(HearingsForTodayByAllocationRequestV2 request)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetHearingsForTodayByCsosV2Async(HearingsForTodayByAllocationRequestV2 request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingNotificationResponseV2>> GetHearingsForNotificationAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingNotificationResponseV2>> GetHearingsForNotificationAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ParticipantResponseV2>> AddParticipantsToHearingV2Async(Guid hearingId, AddParticipantsToHearingRequestV2 request)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ParticipantResponseV2>> AddParticipantsToHearingV2Async(Guid hearingId, AddParticipantsToHearingRequestV2 request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipantResponseV2> UpdateParticipantDetailsV2Async(Guid hearingId, Guid participantId, UpdateParticipantRequestV2 request)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipantResponseV2> UpdateParticipantDetailsV2Async(Guid hearingId, Guid participantId, UpdateParticipantRequestV2 request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ParticipantResponseV2>> UpdateHearingParticipantsV2Async(Guid hearingId, UpdateHearingParticipantsRequestV2 request)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ParticipantResponseV2>> UpdateHearingParticipantsV2Async(Guid hearingId, UpdateHearingParticipantsRequestV2 request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingRoleResponseV2>> GetHearingRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingRoleResponseV2>> GetHearingRolesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponseV2> BookNewHearingWithCodeAsync(BookNewHearingRequestV2 request)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponseV2> BookNewHearingWithCodeAsync(BookNewHearingRequestV2 request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponseV2> GetHearingDetailsByIdV2Async(Guid hearingId)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponseV2> GetHearingDetailsByIdV2Async(Guid hearingId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponseV2> UpdateHearingDetailsV2Async(Guid hearingId, UpdateHearingRequestV2 request)
        {
            throw new NotImplementedException();
        }

        public Task<HearingDetailsResponseV2> UpdateHearingDetailsV2Async(Guid hearingId, UpdateHearingRequestV2 request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetHearingsByGroupIdV2Async(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetHearingsByGroupIdV2Async(Guid groupId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateHearingsInGroupV2Async(Guid groupId, UpdateHearingsInGroupRequestV2 request)
        {
            throw new NotImplementedException();
        }

        public Task UpdateHearingsInGroupV2Async(Guid groupId, UpdateHearingsInGroupRequestV2 request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> CloneHearingAsync(Guid hearingId, CloneHearingRequestV2 request)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> CloneHearingAsync(Guid hearingId, CloneHearingRequestV2 request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<InterpreterLanguagesResponse>> GetAvailableInterpreterLanguagesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<InterpreterLanguagesResponse>> GetAvailableInterpreterLanguagesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<JudiciaryParticipantResponse>> AddJudiciaryParticipantsToHearingAsync(Guid hearingId, IEnumerable<JudiciaryParticipantRequest> request)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<JudiciaryParticipantResponse>> AddJudiciaryParticipantsToHearingAsync(Guid hearingId, IEnumerable<JudiciaryParticipantRequest> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveJudiciaryParticipantFromHearingAsync(Guid hearingId, string personalCode)
        {
            throw new NotImplementedException();
        }

        public Task RemoveJudiciaryParticipantFromHearingAsync(Guid hearingId, string personalCode,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<JudiciaryParticipantResponse> UpdateJudiciaryParticipantAsync(Guid hearingId, string personalCode, UpdateJudiciaryParticipantRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<JudiciaryParticipantResponse> UpdateJudiciaryParticipantAsync(Guid hearingId, string personalCode, UpdateJudiciaryParticipantRequest request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<JudiciaryParticipantResponse> ReassignJudiciaryJudgeAsync(Guid hearingId, ReassignJudiciaryJudgeRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<JudiciaryParticipantResponse> ReassignJudiciaryJudgeAsync(Guid hearingId, ReassignJudiciaryJudgeRequest request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PersonResponseV2>> SearchForPersonV2Async(SearchTermRequestV2 term)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PersonResponseV2>> SearchForPersonV2Async(SearchTermRequestV2 term, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PersonResponseV2> SearchForNonJudgePersonsByContactEmailV2Async(string contactEmail)
        {
            throw new NotImplementedException();
        }

        public Task<PersonResponseV2> SearchForNonJudgePersonsByContactEmailV2Async(string contactEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePersonDetailsV2Async(Guid personId, UpdatePersonDetailsRequestV2 payload)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePersonDetailsV2Async(Guid personId, UpdatePersonDetailsRequestV2 payload,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetUnallocatedHearingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetUnallocatedHearingsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetUnallocatedHearings2Async()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HearingDetailsResponseV2>> GetUnallocatedHearings2Async(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}