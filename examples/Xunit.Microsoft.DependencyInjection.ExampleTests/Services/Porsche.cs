namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services;

internal class Porsche : ICarMaker
{
    public string Manufacturer =>nameof(Porsche);
}