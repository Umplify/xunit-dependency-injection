namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

public interface ICalculator
{
    Task<int> Add(int x, int y);
}
