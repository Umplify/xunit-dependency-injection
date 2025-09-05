using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Example class that demonstrates constructor injection pattern
/// This class is NOT a test class itself, but rather a class that can be instantiated
/// with constructor injection for use in tests
/// </summary>
public class CalculatorService
{
    private readonly ICalculator _calculator;
    private readonly Services.Options _options;
    private readonly ICarMaker _porsche;
    private readonly ICarMaker _toyota;

    internal CalculatorService(
        ICalculator calculator,
        IOptions<Services.Options> options,
        [FromKeyedService("Porsche")] ICarMaker porsche,
        [FromKeyedService("Toyota")] ICarMaker toyota)
    {
        _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _porsche = porsche ?? throw new ArgumentNullException(nameof(porsche));
        _toyota = toyota ?? throw new ArgumentNullException(nameof(toyota));
    }

    public async Task<int> CalculateAsync(int x, int y)
    {
        return await _calculator.AddAsync(x, y);
    }

    public string GetPorscheInfo() => $"Manufacturer: {_porsche.Manufacturer}";
    public string GetToyotaInfo() => $"Manufacturer: {_toyota.Manufacturer}";
    public int GetRate() => _options.Rate;
}