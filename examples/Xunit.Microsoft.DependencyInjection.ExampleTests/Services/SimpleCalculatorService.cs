using Microsoft.Extensions.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Simplified version of CalculatorService without keyed dependencies for debugging
/// </summary>
public class SimpleCalculatorService
{
    private readonly ICalculator _calculator;
    private readonly Services.Options _options;

    internal SimpleCalculatorService(
        ICalculator calculator,
        IOptions<Services.Options> options)
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