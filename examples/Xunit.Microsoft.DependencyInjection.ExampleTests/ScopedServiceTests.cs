using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options = Xunit.Microsoft.DependencyInjection.ExampleTests.Services.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Example tests demonstrating scoped service injection using property injection pattern
/// Scoped services maintain the same instance within a single test scope
/// </summary>
public class ScopedServiceTests : TestBedWithDI<TestProjectFixture>
{
    [Inject]
    private IScopedService? ScopedService1 { get; set; }

    [Inject]
    private IScopedService? ScopedService2 { get; set; }

    [Inject]
    private IOptions<Options>? Options { get; set; }

    public ScopedServiceTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture)
        : base(testOutputHelper, fixture)
    {
    }

    [Fact]
    public async Task TestScopedServiceMaintainsSameInstanceWithinScope()
    {
        // Arrange - Both injected properties should reference the same scoped instance
        Assert.NotNull(ScopedService1);
        Assert.NotNull(ScopedService2);

        // Act - Modify state through first service
        var initialCounter = ScopedService1.Counter;
        ScopedService1.Increment();
        ScopedService1.Increment();

        // Assert - Both references should have the same instance ID and counter value
        Assert.Equal(ScopedService1.InstanceId, ScopedService2.InstanceId);
        Assert.Equal(initialCounter + 2, ScopedService1.Counter);
        Assert.Equal(initialCounter + 2, ScopedService2.Counter); // Same instance, same counter

        // Process data through second reference
        var result = await ScopedService2.ProcessAsync("test data");
        Assert.Contains(ScopedService1.InstanceId.ToString(), result);
        Assert.Contains($"counter: {initialCounter + 2}", result);
    }

    [Fact]
    public void TestScopedServiceFromServiceProvider()
    {
        // Arrange - Get scoped service through service provider
        var scopedFromProvider = GetService<IScopedService>();

        // Assert - Should be the same instance as the injected property
        Assert.NotNull(ScopedService1);
        Assert.NotNull(scopedFromProvider);
        Assert.Equal(ScopedService1.InstanceId, scopedFromProvider.InstanceId);
    }

    [Theory]
    [InlineData("data1")]
    [InlineData("data2")]
    public async Task TestScopedServiceStateConsistency(string testData)
    {
        // Arrange
        Assert.NotNull(ScopedService1);
        var initialCounter = ScopedService1.Counter;

        // Act
        ScopedService1.Increment();
        var result = await ScopedService1.ProcessAsync(testData);

        // Assert - Counter should have incremented by 1
        Assert.Equal(initialCounter + 1, ScopedService1.Counter);
        Assert.Contains(testData, result);
        Assert.Contains(ScopedService1.InstanceId.ToString(), result);
    }

    [Fact]
    public void TestScopedServiceWithFuncFactory()
    {
        // Arrange - Create Func<IScopedService> factory
        var serviceFactory = GetService<Func<IScopedService>>();
        Assert.NotNull(serviceFactory);

        // Act - Create service through factory
        var serviceThroughFactory = serviceFactory();

        // Assert - Should be the same scoped instance
        Assert.NotNull(ScopedService1);
        Assert.NotNull(serviceThroughFactory);
        Assert.Equal(ScopedService1.InstanceId, serviceThroughFactory.InstanceId);
    }
}