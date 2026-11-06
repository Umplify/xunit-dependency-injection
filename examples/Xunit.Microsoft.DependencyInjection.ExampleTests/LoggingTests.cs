using Microsoft.Extensions.Logging;
using Xunit.Microsoft.DependencyInjection.Logging;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// The fixture builds its container once, when the first test in the class resolves a service.
/// Loggers resolved from that container must still write to the output of whichever test is
/// running when they log - not to the test that happened to build the container.
/// </summary>
public class LoggingTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture)
    : TestBed<TestProjectFixture>(testOutputHelper, fixture)
{
    [Fact]
    public void FirstTestLogsToItsOwnOutput() => AssertLogReachesCurrentTest();

    [Fact]
    public void SecondTestLogsToItsOwnOutput() => AssertLogReachesCurrentTest();

    [Fact]
    public void ThirdTestLogsToItsOwnOutput() => AssertLogReachesCurrentTest();

    /// <summary>
    /// A logger built directly from the public constructor keeps writing to the helper it was given.
    /// </summary>
    [Fact]
    public void DirectlyConstructedLoggerWritesToItsOwnHelper()
    {
        var helper = new RecordingOutputHelper();

        new OutputLogger("Direct", helper).LogInformation("direct message");

        Assert.Contains("direct message", helper.Output);
        Assert.DoesNotContain("direct message", TestContext.Current.TestOutputHelper!.Output);
    }

    private sealed class RecordingOutputHelper : ITestOutputHelper
    {
        private readonly System.Text.StringBuilder _output = new();

        public string Output => _output.ToString();

        public void Write(string message) => _output.Append(message);

        public void Write(string format, params object[] args) => _output.AppendFormat(format, args);

        public void WriteLine(string message) => _output.AppendLine(message);

        public void WriteLine(string format, params object[] args) => _output.AppendFormat(format, args).AppendLine();
    }

    private void AssertLogReachesCurrentTest()
    {
        var logger = _fixture.GetService<ILogger<LoggingTests>>(_testOutputHelper)!;
        var message = $"log-{Guid.NewGuid():N}";

        logger.LogInformation("{Message}", message);

        Assert.Contains(message, TestContext.Current.TestOutputHelper!.Output);
    }
}
