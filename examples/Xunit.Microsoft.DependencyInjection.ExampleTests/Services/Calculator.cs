using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

public class Calculator(ILogger<Calculator> logger, IOptions<Options> option) : ICalculator
{
    private readonly Options _option = option.Value;
    private readonly ILogger<Calculator> _logger = logger;

    public Task<int> AddAsync(int x, int y)
    {
        var result = (x + y) * _option.Rate;
        _logger.LogInformation("The result is {@Result}", result);
        return Task.FromResult(result);
    }
}
