using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit.Microsoft.DependencyInjection.ExampleTests.CalculationService;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

public class RemoteCalculator(ILogger<Calculator> logger, IOptions<Options> option, IHttpClientFactory factory) : ICalculator
{
    private readonly Options _option = option.Value;

    public async Task<int> AddAsync(int x, int y)
    {
        var addRequestJson = JsonSerializer.Serialize(new AddRequest(x, y));
        var content = new StringContent(addRequestJson, Encoding.UTF8, "application/json");
        var response = await factory.CreateClient().PostAsync("add", content);
        var body = await new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8).ReadToEndAsync();
        var result = int.Parse(body) * _option.Rate;
        logger.LogInformation("The result is {@Result}", result);
        return result;
    }
}