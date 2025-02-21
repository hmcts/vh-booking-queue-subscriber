using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Common.Logging;

[ExcludeFromCodeCoverage]
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