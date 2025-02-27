using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;

namespace BookingQueueSubscriber;

[ExcludeFromCodeCoverage] // for now
public class ServiceBusListener(
    IMessageHandlerFactory messageHandlerFactory,
    ServiceBusProcessor serviceBusProcessor,
    ILogger<ServiceBusListener> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        serviceBusProcessor.ProcessMessageAsync += MessageHandler;
        serviceBusProcessor.ProcessErrorAsync += ErrorHandler;
        
        logger.LogInformation("Starting service bus processor");
        await serviceBusProcessor.StartProcessingAsync(stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var bookingQueueItem = args.Message.Body.ToString();
        
        logger.LogInformation("Processing message {BookingQueueItem}", bookingQueueItem);
        // get handler
        var eventMessage = MessageSerializer.Deserialise<EventMessage>(bookingQueueItem);

        var handler = messageHandlerFactory.Get(eventMessage.IntegrationEvent);
        logger.LogDebug("using handler {Handler}", handler.GetType());

        await handler.HandleAsync(eventMessage.IntegrationEvent);
        logger.LogInformation("Process message {EventMessageId} - {EventMessageIntegrationEvent}", eventMessage.Id,
            eventMessage.IntegrationEvent);
        
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error processing message");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping service bus processor");
        await serviceBusProcessor.StopProcessingAsync(cancellationToken);
        await serviceBusProcessor.DisposeAsync();
    }
}