namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

/// <summary>
/// Interface for demonstrating singleton service injection
/// </summary>
public interface ISingletonService
{
    Guid InstanceId { get; }
    DateTime CreatedAt { get; }
    int GlobalCounter { get; }
    void IncrementGlobal();
    Task<string> GetStatusAsync();
}