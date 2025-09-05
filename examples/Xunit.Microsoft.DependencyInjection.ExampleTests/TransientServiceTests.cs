using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options = Xunit.Microsoft.DependencyInjection.ExampleTests.Services.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Example tests demonstrating transient service injection using property injection pattern
/// Transient services create a new instance for each injection point
/// </summary>
public class TransientServiceTests : TestBedWithDI<TestProjectFixture>
{
    [Inject]
    private ITransientService? TransientService1 { get; set; }

    [Inject]
    private ITransientService? TransientService2 { get; set; }

    [Inject]
    private ICalculator? Calculator { get; set; } // Calculator is also transient

    [Inject]
    private IOptions<Options>? Options { get; set; }

    public TransientServiceTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture)
        : base(testOutputHelper, fixture)
    {
    }

    [Fact]
    public async Task TestTransientServiceCreatesDifferentInstances()
    {
        // Arrange - Each injected property should have a different transient instance
        Assert.NotNull(TransientService1);
        Assert.NotNull(TransientService2);

        // Assert - Should be different instances with different IDs
        Assert.NotEqual(TransientService1.InstanceId, TransientService2.InstanceId);

        // Different creation times (though might be very close)
        // We can't guarantee different times due to fast execution, but we can verify different instances

        // Act - Process data through both services
        var result1 = await TransientService1.ProcessDataAsync("test1");
        var result2 = await TransientService2.ProcessDataAsync("test2");

        // Assert - Results should contain different instance IDs
        Assert.Contains(TransientService1.InstanceId.ToString(), result1);
        Assert.Contains(TransientService2.InstanceId.ToString(), result2);
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void TestTransientServiceFromServiceProvider()
    {
        // Arrange - Get transient services through service provider
        var transientFromProvider1 = GetService<ITransientService>();
        var transientFromProvider2 = GetService<ITransientService>();

        // Assert - Should be different instances from each other and from injected properties
        Assert.NotNull(TransientService1);
        Assert.NotNull(transientFromProvider1);
        Assert.NotNull(transientFromProvider2);

        Assert.NotEqual(TransientService1.InstanceId, transientFromProvider1.InstanceId);
        Assert.NotEqual(TransientService1.InstanceId, transientFromProvider2.InstanceId);
        Assert.NotEqual(transientFromProvider1.InstanceId, transientFromProvider2.InstanceId);
    }

    [Theory]
    [InlineData("input1", "input2")]
    [InlineData("data-a", "data-b")]
    public async Task TestTransientServiceIndependentOperation(string data1, string data2)
    {
        // Arrange
        Assert.NotNull(TransientService1);
        Assert.NotNull(TransientService2);

        // Act - Process different data with different instances
        var result1 = await TransientService1.ProcessDataAsync(data1);
        var result2 = await TransientService2.ProcessDataAsync(data2);

        var hash1 = TransientService1.CalculateHash(data1);
        var hash2 = TransientService2.CalculateHash(data2);

        // Assert - Results should be independent and contain respective instance IDs
        Assert.Contains(data1, result1);
        Assert.Contains(data2, result2);
        Assert.Contains(TransientService1.InstanceId.ToString(), result1);
        Assert.Contains(TransientService2.InstanceId.ToString(), result2);

        // Hash calculations should be independent
        Assert.Equal(data1.GetHashCode(), hash1);
        Assert.Equal(data2.GetHashCode(), hash2);
    }

    [Fact]
    public void TestTransientServiceWithFuncFactory()
    {
        // Arrange - Create Func<ITransientService> factory
        var serviceFactory = GetService<Func<ITransientService>>();
        Assert.NotNull(serviceFactory);

        // Act - Create multiple services through factory
        var serviceThroughFactory1 = serviceFactory();
        var serviceThroughFactory2 = serviceFactory();

        // Assert - Should be different instances each time
        Assert.NotNull(TransientService1);
        Assert.NotNull(serviceThroughFactory1);
        Assert.NotNull(serviceThroughFactory2);

        Assert.NotEqual(TransientService1.InstanceId, serviceThroughFactory1.InstanceId);
        Assert.NotEqual(TransientService1.InstanceId, serviceThroughFactory2.InstanceId);
        Assert.NotEqual(serviceThroughFactory1.InstanceId, serviceThroughFactory2.InstanceId);
    }

    [Fact]
    public async Task TestTransientServiceWithActionDelegate()
    {
        // Arrange - Create action delegates to work with transient services
        var results = new List<string>();
        var instanceIds = new List<Guid>();

        Action<ITransientService> serviceAction = service =>
        {
            instanceIds.Add(service.InstanceId);
            var hash = service.CalculateHash($"action-{instanceIds.Count}");
            results.Add($"Action {instanceIds.Count}: Hash={hash}, Instance={service.InstanceId}");
        };

        // Act - Execute actions with different transient instances
        Assert.NotNull(TransientService1);
        Assert.NotNull(TransientService2);

        serviceAction(TransientService1);
        serviceAction(TransientService2);

        // Get a new transient and execute action
        var newTransient = GetService<ITransientService>();
        serviceAction(newTransient);

        // Assert - Should have worked with 3 different instances
        Assert.Equal(3, results.Count);
        Assert.Equal(3, instanceIds.Count);
        Assert.True(instanceIds.Distinct().Count() == 3); // All different

        // Verify results contain expected patterns
        Assert.All(results, result =>
        {
            Assert.Contains("Action", result);
            Assert.Contains("Hash=", result);
            Assert.Contains("Instance=", result);
        });
    }

    [Fact]
    public async Task TestMixedTransientServicesWithCalculator()
    {
        // Arrange - Calculator is also transient, should be different from our transient services
        Assert.NotNull(Calculator);
        Assert.NotNull(TransientService1);
        Assert.NotNull(Options);

        // Act - Use both transient services
        var calculatorResult = await Calculator.AddAsync(10, 5);
        var serviceResult = await TransientService1.ProcessDataAsync("calculator-test");
        var serviceHash = TransientService1.CalculateHash("hash-test");

        // Assert - Results should be independent
        var expectedCalculatorResult = Options.Value.Rate * (10 + 5);
        Assert.Equal(expectedCalculatorResult, calculatorResult);

        Assert.Contains("calculator-test", serviceResult);
        Assert.Contains(TransientService1.InstanceId.ToString(), serviceResult);

        Assert.Equal("hash-test".GetHashCode(), serviceHash);
    }
}