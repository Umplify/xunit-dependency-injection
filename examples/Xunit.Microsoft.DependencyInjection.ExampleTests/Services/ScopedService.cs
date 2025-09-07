using Microsoft.Extensions.Logging;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

/// <summary>
/// Scoped service implementation that maintains state within a scope
/// </summary>
public class ScopedService : IScopedService
{
    private readonly ILogger<ScopedService> _logger;
    private int _counter = 0;

    public Guid InstanceId { get; } = Guid.NewGuid();
    public int Counter => _counter;

    public ScopedService(ILogger<ScopedService> logger)
    {
        _logger = logger;
        _logger.LogInformation("ScopedService created with InstanceId: {InstanceId}", InstanceId);
    }

    public void Increment()
    {
        _counter++;
        _logger.LogInformation("Counter incremented to {Counter} for InstanceId: {InstanceId}", _counter, InstanceId);
    }

    public Task<string> ProcessAsync(string input)
    {
        var result = $"Processed '{input}' by instance {InstanceId} (counter: {_counter})";
        _logger.LogInformation("Processing result: {Result}", result);
        return Task.FromResult(result);
    }
}