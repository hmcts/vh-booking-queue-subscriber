using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;

namespace BookingQueueSubscriber;

[ExcludeFromCodeCoverage] // for now
public class ServiceBusListener : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IMessageHandlerFactory _messageHandlerFactory;
    private readonly ILogger<ServiceBusListener> _logger;

    public ServiceBusListener(IMessageHandlerFactory messageHandlerFactory, ILogger<ServiceBusListener> logger)
    {
        _messageHandlerFactory = messageHandlerFactory;
        _logger = logger;
        var client = new ServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusConnection"));
        _processor = client.CreateProcessor(Environment.GetEnvironmentVariable("queueName"), new ServiceBusProcessorOptions());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var bookingQueueItem = args.Message.Body.ToString();
        
        _logger.LogInformation("Processing message {BookingQueueItem}", bookingQueueItem);
        // get handler
        var eventMessage = MessageSerializer.Deserialise<EventMessage>(bookingQueueItem);

        var handler = _messageHandlerFactory.Get(eventMessage.IntegrationEvent);
        _logger.LogDebug("using handler {Handler}", handler.GetType());

        await handler.HandleAsync(eventMessage.IntegrationEvent);
        _logger.LogInformation("Process message {EventMessageId} - {EventMessageIntegrationEvent}", eventMessage.Id,
            eventMessage.IntegrationEvent);
        
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing message");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
    }
}