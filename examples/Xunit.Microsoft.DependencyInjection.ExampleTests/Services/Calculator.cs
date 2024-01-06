using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

public class Calculator : ICalculator
{
    private readonly Options _option;
    private readonly ILogger<Calculator> _logger;

    public Calculator(ILogger<Calculator> logger, IOptions<Options> option)
        => (_logger, _option) = (logger, option.Value);

    public Task<int> Add(int x, int y)
    {
        var result = (x + y) * _option.Rate;
        _logger.LogInformation("The result is {@Result}", result);
        return Task.FromResult(result);
    }
}
