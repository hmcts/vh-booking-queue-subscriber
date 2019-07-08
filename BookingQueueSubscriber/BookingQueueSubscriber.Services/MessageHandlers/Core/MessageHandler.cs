using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;

namespace BookingQueueSubscriber.Services.MessageHandlers.Core
{
    public interface IMessageHandler<in T> : IMessageHandler where T : IIntegrationEvent
    {
        Task HandleAsync(T eventMessage);
    }

    public interface IMessageHandler
    {
        Task HandleAsync(object integrationEvent);
    }
}