using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Xunit.Microsoft.DependencyInjection.Logging
{
    public class OutputLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public OutputLoggerProvider(ITestOutputHelper testOutputHelper)
            => _testOutputHelper = testOutputHelper;

        public ILogger CreateLogger(string categoryName)
            => new OutputLogger(categoryName, _testOutputHelper);

        public void Dispose()
        {
        }
    }
}
