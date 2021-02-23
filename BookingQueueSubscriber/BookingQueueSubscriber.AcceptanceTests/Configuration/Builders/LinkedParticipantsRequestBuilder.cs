using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingsApi.Contract.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookingsApi.Contract.Enums;

namespace BookingQueueSubscriber.AcceptanceTests.Configuration.Builders
{
    public class LinkedParticipantsRequestBuilder
    {

        private List<LinkedParticipantRequest> linkedParticipantRequest;

        public LinkedParticipantsRequestBuilder(IList<ParticipantRequest> participants)
        {
            linkedParticipantRequest = new List<LinkedParticipantRequest>();
            var participant = participants.FirstOrDefault(p => p.HearingRoleName == RoleData.INDV_HEARING_ROLE_NAME);
            var interpreter = participants.FirstOrDefault(p => p.HearingRoleName == RoleData.INTERPRETER_HEARING_ROLE_NAME);
            linkedParticipantRequest.Add(Create(participant.ContactEmail,interpreter.ContactEmail));
            linkedParticipantRequest.Add(Create(interpreter.ContactEmail, participant.ContactEmail));
        }

        public LinkedParticipantsRequestBuilder(string participantEmail, string interpreterEmail)
        {
            linkedParticipantRequest = new List<LinkedParticipantRequest>();
            linkedParticipantRequest.Add(Create(participantEmail, interpreterEmail));
            linkedParticipantRequest.Add(Create(interpreterEmail, participantEmail));
        }

        private LinkedParticipantRequest Create(string participantEmail,string linkedParticipantEmail )
        { 
            var request = new LinkedParticipantRequest();
            request.ParticipantContactEmail = participantEmail;
            request.LinkedParticipantContactEmail = linkedParticipantEmail;
            request.Type = LinkedParticipantType.Interpreter;
            return request;
        }


        public List<LinkedParticipantRequest> Build()
        {
            return linkedParticipantRequest;
        }
    }
}
