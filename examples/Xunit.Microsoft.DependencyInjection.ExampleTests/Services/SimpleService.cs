using Microsoft.Extensions.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Simple service class without keyed services for testing
/// </summary>
public class SimpleService
{
    private readonly ICalculator _calculator;
    private readonly Services.Options _options;

    public SimpleService(ICalculator calculator, IOptions<Services.Options> options)
    {
        _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<int> CalculateAsync(int x, int y)
    {
        return await _calculator.AddAsync(x, y);
    }

    public int GetRate() => _options.Rate;
}