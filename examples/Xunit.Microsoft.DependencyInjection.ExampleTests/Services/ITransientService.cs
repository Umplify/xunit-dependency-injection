namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

/// <summary>
/// Interface for demonstrating transient service injection
/// </summary>
public interface ITransientService
{
    Guid InstanceId { get; }
    DateTime CreatedAt { get; }
    Task<string> ProcessDataAsync(string data);
    int CalculateHash(string input);
}