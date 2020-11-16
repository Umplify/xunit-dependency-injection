using Microsoft.Extensions.Logging;

namespace Xunit.Microsoft.DependencyInjection.Logging
{
    public sealed class NilLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
            => null;

        public void Dispose()
        {
        }
    }
}
