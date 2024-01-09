namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

public interface ICalculator
{
    Task<int> AddAsync(int x, int y);
}
