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
        public List<AddNotificationRequest> NotificationRequests { get; set; } = new List<AddNotificationRequest>();
        public Task<HealthResponse> CheckServiceHealthAuth2Async()
        {
            throw new NotImplementedException();
        }

        public Task<HealthResponse> CheckServiceHealthAuth2Async(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HealthResponse> CheckServiceHealthAuthAsync()
        {
            throw new NotImplementedException();
        }

        public Task<HealthResponse> CheckServiceHealthAuthAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CreateNewNotificationAsync(AddNotificationRequest request)
        {
            NotificationRequests.Add(request);
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task CreateNewNotificationAsync(AddNotificationRequest request, CancellationToken cancellationToken)
        {
            NotificationRequests.Add(request);
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
    }
}
