using System.Diagnostics.CodeAnalysis;
using System.Net;
using NotificationApi.Client;
using NotificationApi.Contract;
using NotificationApi.Contract.Requests;
using NotificationApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    [ExcludeFromCodeCoverage]
    public class NotificationApiClientFake : INotificationApiClient
    {
        public List<Object> NotificationRequests { get; set; } = new List<Object>();
        public Task CreateNewNotificationAsync(AddNotificationRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task CreateNewNotificationAsync(AddNotificationRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task<NotificationTemplateResponse> GetTemplateByNotificationTypeAsync(NotificationType notificationType)
        {
            throw new NotImplementedException();
        }

        public Task<NotificationTemplateResponse> GetTemplateByNotificationTypeAsync(NotificationType notificationType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task HandleCallbackAsync(NotificationCallbackRequest notificationCallbackRequest)
        {
            throw new NotImplementedException();
        }

        public Task HandleCallbackAsync(NotificationCallbackRequest notificationCallbackRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SendHearingAmendmentEmailAsync(HearingAmendmentRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendHearingAmendmentEmailAsync(HearingAmendmentRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendMultiDayHearingReminderEmailAsync(MultiDayHearingReminderRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendMultiDayHearingReminderEmailAsync(MultiDayHearingReminderRequest request, CancellationToken cancellationToken)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantCreatedAccountEmailAsync(SignInDetailsEmailRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantCreatedAccountEmailAsync(SignInDetailsEmailRequest request, CancellationToken cancellationToken)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantMultiDayHearingConfirmationForExistingUserEmailAsync(ExistingUserMultiDayHearingConfirmationRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantMultiDayHearingConfirmationForExistingUserEmailAsync(ExistingUserMultiDayHearingConfirmationRequest request, CancellationToken cancellationToken)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantMultiDayHearingConfirmationForNewUserEmailAsync(NewUserMultiDayHearingConfirmationRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantMultiDayHearingConfirmationForNewUserEmailAsync(NewUserMultiDayHearingConfirmationRequest request, CancellationToken cancellationToken)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(ExistingUserSingleDayHearingConfirmationRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(ExistingUserSingleDayHearingConfirmationRequest request, CancellationToken cancellationToken)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantSingleDayHearingConfirmationForNewUserEmailAsync(NewUserSingleDayHearingConfirmationRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantSingleDayHearingConfirmationForNewUserEmailAsync(NewUserSingleDayHearingConfirmationRequest request, CancellationToken cancellationToken)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantWelcomeEmailAsync(NewUserWelcomeEmailRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendParticipantWelcomeEmailAsync(NewUserWelcomeEmailRequest request, CancellationToken cancellationToken)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendResetPasswordEmailAsync(PasswordResetEmailRequest request)
        {
            throw new NotImplementedException();
        }

        public Task SendResetPasswordEmailAsync(PasswordResetEmailRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SendSingleDayHearingReminderEmailAsync(SingleDayHearingReminderRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task SendSingleDayHearingReminderEmailAsync(SingleDayHearingReminderRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
