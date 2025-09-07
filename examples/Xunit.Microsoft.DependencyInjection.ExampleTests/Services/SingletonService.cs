using Microsoft.Extensions.Logging;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

/// <summary>
/// Singleton service implementation that maintains global state
/// </summary>
public class SingletonService : ISingletonService
{
    private readonly ILogger<SingletonService> _logger;
    private int _globalCounter = 0;

    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public int GlobalCounter => _globalCounter;

    public SingletonService(ILogger<SingletonService> logger)
    {
        _logger = logger;
        _logger.LogInformation("SingletonService created with InstanceId: {InstanceId} at {CreatedAt}", InstanceId, CreatedAt);
    }

    public void IncrementGlobal()
    {
        _globalCounter++;
        _logger.LogInformation("Global counter incremented to {GlobalCounter} for InstanceId: {InstanceId}", _globalCounter, InstanceId);
    }

    public Task<string> GetStatusAsync()
    {
        var uptime = DateTime.UtcNow - CreatedAt;
        var result = $"Singleton {InstanceId} - Created: {CreatedAt:yyyy-MM-dd HH:mm:ss} UTC, Uptime: {uptime:c}, Global Counter: {_globalCounter}";
        _logger.LogInformation("Status requested: {Result}", result);
        return Task.FromResult(result);
    }
}