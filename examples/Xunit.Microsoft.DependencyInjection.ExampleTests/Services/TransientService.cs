using Microsoft.Extensions.Logging;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

/// <summary>
/// Transient service implementation - new instance created for each injection
/// </summary>
public class TransientService : ITransientService
{
    private readonly ILogger<TransientService> _logger;

    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public TransientService(ILogger<TransientService> logger)
    {
        _logger = logger;
        _logger.LogInformation("TransientService created with InstanceId: {InstanceId} at {CreatedAt}", InstanceId, CreatedAt);
    }

    public Task<string> ProcessDataAsync(string data)
    {
        var result = $"Data '{data}' processed by transient instance {InstanceId} at {CreatedAt:HH:mm:ss.fff}";
        _logger.LogInformation("Processing data: {Result}", result);
        return Task.FromResult(result);
    }

    public int CalculateHash(string input)
    {
        var hash = input?.GetHashCode() ?? 0;
        _logger.LogInformation("Calculated hash {Hash} for input '{Input}' by instance {InstanceId}", hash, input, InstanceId);
        return hash;
    }
}