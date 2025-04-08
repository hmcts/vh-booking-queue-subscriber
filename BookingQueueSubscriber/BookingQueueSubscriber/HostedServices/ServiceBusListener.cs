using Azure.Messaging.ServiceBus;
using BookingQueueSubscriber.Wrappers;
using Microsoft.Extensions.Hosting;

namespace BookingQueueSubscriber.HostedServices;

public class ServiceBusListener(
    IMessageHandlerFactory messageHandlerFactory,
    IServiceBusProcessorWrapper serviceBusProcessor,
    ILogger<ServiceBusListener> logger)
    : BackgroundService
{
    public async Task HandleMessage(ProcessMessageEventArgs args)
    {
        var bookingQueueItem = args.Message.Body.ToString();
        
        logger.ProcessingMessage(bookingQueueItem);
        var eventMessage = MessageSerializer.Deserialise<EventMessage>(bookingQueueItem);

        var handler = messageHandlerFactory.Get(eventMessage.IntegrationEvent);
        logger.UsingHandler(handler.GetType());

        await handler.HandleAsync(eventMessage.IntegrationEvent);
        logger.ProcessMessage(eventMessage.Id, eventMessage.IntegrationEvent);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        serviceBusProcessor.ProcessMessageAsync -= HandleMessage;
        serviceBusProcessor.ProcessErrorAsync -= HandleError;
        
        logger.StoppingServiceBusProcessor();
        await serviceBusProcessor.StopProcessingAsync(cancellationToken);
        await serviceBusProcessor.DisposeAsync();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        serviceBusProcessor.ProcessMessageAsync += HandleMessage;
        serviceBusProcessor.ProcessErrorAsync += HandleError;
        
        logger.StartingServiceBusProcessor();
        await serviceBusProcessor.StartProcessingAsync(stoppingToken);
    }
    
    private Task HandleError(ProcessErrorEventArgs args)
    {
        logger.ErrorProcessingMessage(args.Exception);
        return Task.CompletedTask;
    }
}