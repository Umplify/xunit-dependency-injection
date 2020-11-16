using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace Xunit.Microsoft.DependencyInjection.Logging
{
    public class OutputLogger : ILogger
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly string _categoryName;

        public OutputLogger(ITestOutputHelper testOutputHelper)
            : this("Tests", testOutputHelper)
		{
		}

        public OutputLogger(string categoryName, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
            => new NoOpDisposable();

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => _testOutputHelper.WriteLine($"{logLevel} - Category: {_categoryName} : {formatter(state, exception)} :: {DateTime.Now}");

        private class NoOpDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}
