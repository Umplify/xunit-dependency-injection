using Microsoft.Extensions.Logging;

namespace Umplify.Test.Tools.Logging
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
