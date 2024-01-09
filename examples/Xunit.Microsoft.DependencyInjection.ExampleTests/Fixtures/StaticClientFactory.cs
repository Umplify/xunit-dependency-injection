namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

public class StaticClientFactory(HttpClient client) : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return client;
    }
}