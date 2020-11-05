using Microsoft.Extensions.Logging;
using System;

namespace Projektor.WebApp
{
    public class Logger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            ConsoleColor color;
            switch (logLevel)
            {
                case LogLevel.Error:
                    color = ConsoleColor.Red;
                    break;
                case LogLevel.Information:
                    color = ConsoleColor.White;
                    break;
                default:
                    color = ConsoleColor.White;
                    break;
            }

            Console.ForegroundColor = color;
            Console.WriteLine($"{formatter(state, exception)}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
