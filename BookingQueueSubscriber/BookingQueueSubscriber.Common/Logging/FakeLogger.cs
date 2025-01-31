using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Common.Logging;

public class FakeLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Console.WriteLine($"{logLevel}: {formatter(state, exception)}");
    }
}