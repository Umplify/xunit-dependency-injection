namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Example tests demonstrating factory-based constructor injection
/// This approach allows for true constructor injection by creating instances via the fixture factory
/// </summary>
public class FactoryConstructorInjectionTests : TestBed<FactoryTestProjectFixture>
{
    public FactoryConstructorInjectionTests(ITestOutputHelper testOutputHelper, FactoryTestProjectFixture fixture)
        : base(testOutputHelper, fixture)
    {
    }

    [Fact]
    public async Task TestSimpleConstructorInjectionViaFactory()
    {
        // Arrange - Test without keyed services first
        var simpleService = _fixture.CreateTestInstance<SimpleService>(_testOutputHelper);

        // Act
        var result = await simpleService.CalculateAsync(10, 5);
        var rate = simpleService.GetRate();

        // Assert
        var expected = rate * (10 + 5);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestConstructorInjectionViaFactory()
    {
        // Arrange - Create an instance with constructor injection via factory
        var calculatorService = _fixture.CreateTestInstance<CalculatorService>(_testOutputHelper);

        // Act
        var result = await calculatorService.CalculateAsync(10, 5);
        var rate = calculatorService.GetRate();

        // Assert
        var expected = rate * (10 + 5);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestKeyedServicesViaConstructorInjection()
    {
        // Arrange - Create an instance with keyed services injected via constructor
        var calculatorService = _fixture.CreateTestInstance<CalculatorService>(_testOutputHelper);

        // Act & Assert
        Assert.Equal("Manufacturer: Porsche", calculatorService.GetPorscheInfo());
        Assert.Equal("Manufacturer: Toyota", calculatorService.GetToyotaInfo());
    }

    [Theory]
    [InlineData(7, 3)]
    public async Task TestFactoryInjectionWithTheoryData(int x, int y)
    {
        // Arrange
        var calculatorService = _fixture.CreateTestInstance<CalculatorService>(_testOutputHelper);

        // Act
        var result = await calculatorService.CalculateAsync(x, y);
        var rate = calculatorService.GetRate();

        // Assert
        var expected = rate * (x + y);
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Example of creating a test class that requires additional parameters
    /// beyond what DI can provide
    /// </summary>
    [Fact]
    public void TestFactoryWithAdditionalParameters()
    {
        // Create a custom test class that needs both DI services and custom parameters
        var testString = "test-data";
        var testInstance = _fixture.CreateTestInstance<CustomTestClass>(_testOutputHelper, testString);

        Assert.NotNull(testInstance.Calculator);
        Assert.Equal(testString, testInstance.CustomData);
    }
}

/// <summary>
/// Example class that demonstrates constructor injection with both DI services
/// and custom parameters
/// </summary>
public class CustomTestClass
{
    public ICalculator Calculator { get; }
    public string CustomData { get; }

    public CustomTestClass(ICalculator calculator, string customData)
    {
        Calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        CustomData = customData ?? throw new ArgumentNullException(nameof(customData));
    }
}

/// <summary>
/// Debug test to check if factory works with non-keyed services
/// </summary>
public class DebugNonKeyedFactoryTests : TestBed<FactoryTestProjectFixture>
{
    public DebugNonKeyedFactoryTests(ITestOutputHelper testOutputHelper, FactoryTestProjectFixture fixture)
        : base(testOutputHelper, fixture)
    {
    }

    [Fact]
    public async Task TestSimpleCalculatorServiceFactory()
    {
        // Test creating SimpleCalculatorService without keyed dependencies
        var simpleCalculator = _fixture.CreateTestInstance<SimpleCalculatorService>(_testOutputHelper);
        
        // Act
        var result = await simpleCalculator.CalculateAsync(10, 5);
        var rate = simpleCalculator.GetRate();

        // Assert
        var expected = rate * (10 + 5);
        Assert.Equal(expected, result);
    }
}