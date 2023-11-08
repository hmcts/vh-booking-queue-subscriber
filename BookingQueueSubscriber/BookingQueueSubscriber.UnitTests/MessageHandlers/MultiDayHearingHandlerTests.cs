using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Enums;
using BookingsApi.Contract.V1.Enums;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class MultiDayHearingHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_SendHearingAmendmentNotificationAsync_when_request_is_valid()
        {
            var messageHandler = new MultiDayHearingHandler(NotificationApiClientMock.Object);
            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            NotificationApiClientMock.Verify(x => x.SendParticipantMultiDayHearingConfirmationForExistingUserEmailAsync(It.IsAny<ExistingUserMultiDayHearingConfirmationRequest>()));
        }

        [Test]
        public async Task should_call_SendHearingAmendmentNotificationAsync_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler)new MultiDayHearingHandler(NotificationApiClientMock.Object);
            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            var participant = integrationEvent.HearingConfirmationForParticipant;

            NotificationApiClientMock.Verify(x => x.SendParticipantMultiDayHearingConfirmationForExistingUserEmailAsync(It.Is<ExistingUserMultiDayHearingConfirmationRequest>(
                            request =>
                                request.TotalDays == integrationEvent.TotalDays &&
                                request.Username == participant.Username &&
                                request.ContactEmail == participant.ContactEmail &&
                                request.RoleName == participant.UserRole &&
                                request.DisplayName == participant.DisplayName &&
                                request.Representee == participant.Representee
                        )), Times.Once);
        }
        private MultiDayHearingIntegrationEvent GetIntegrationEvent()
        {

            return new MultiDayHearingIntegrationEvent
            {
                HearingConfirmationForParticipant = new HearingConfirmationForParticipantDto
                {
                    CaseName = "Case 123",
                    CaseNumber = "Case Number 123",
                    ContactEmail = "participant1@test.com",
                    ContactTelephone = "12345566",
                    DisplayName = "displayname",
                    FirstName = "firstname",
                    LastName = "lastname",
                    HearingId = Guid.NewGuid(),
                    ParticipantId = Guid.NewGuid(),
                    Representee = "",
                    ScheduledDateTime = DateTime.Now.AddDays(1),
                    Username = "first.last@test.com",
                    UserRole = UserRole.Individual.ToString()
                }
            };
        }
    }
}