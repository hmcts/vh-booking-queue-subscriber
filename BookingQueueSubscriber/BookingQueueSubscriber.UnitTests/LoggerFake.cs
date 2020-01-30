using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.UnitTests
{
    public class LoggerFake : ILogger
    {
        public List<string> Messages { get;}

        public LoggerFake()
        {
            Messages = new List<string>();
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Messages.Add(state.ToString());
            Console.WriteLine(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
