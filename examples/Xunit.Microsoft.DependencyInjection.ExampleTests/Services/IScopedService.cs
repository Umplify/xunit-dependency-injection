namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

/// <summary>
/// Interface for demonstrating scoped service injection
/// </summary>
public interface IScopedService
{
    Guid InstanceId { get; }
    int Counter { get; }
    void Increment();
    Task<string> ProcessAsync(string input);
}