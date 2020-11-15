using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Umplify.Test.Tools.Logging
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
