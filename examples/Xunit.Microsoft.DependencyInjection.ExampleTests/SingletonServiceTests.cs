using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options = Xunit.Microsoft.DependencyInjection.ExampleTests.Services.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Example tests demonstrating singleton service injection using property injection pattern
/// Singleton services maintain the same instance across all tests and scopes
/// </summary>
public class SingletonServiceTests : TestBedWithDI<TestProjectFixture>
{
    [Inject]
    private ISingletonService? SingletonService1 { get; set; }

    [Inject]
    private ISingletonService? SingletonService2 { get; set; }

    [Inject]
    private IOptions<Options>? Options { get; set; }

    public SingletonServiceTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture)
        : base(testOutputHelper, fixture)
    {
    }

    [Fact]
    public async Task TestSingletonServiceMaintainsSameInstanceAcrossInjections()
    {
        // Arrange - Both injected properties should reference the same singleton instance
        Assert.NotNull(SingletonService1);
        Assert.NotNull(SingletonService2);

        // Assert - Should be the exact same instance
        Assert.Equal(SingletonService1.InstanceId, SingletonService2.InstanceId);
        Assert.Equal(SingletonService1.CreatedAt, SingletonService2.CreatedAt);

        // Act - Increment through first reference
        var initialCounter = SingletonService1.GlobalCounter;
        SingletonService1.IncrementGlobal();
        SingletonService1.IncrementGlobal();

        // Assert - Both references should reflect the same updated state
        Assert.Equal(initialCounter + 2, SingletonService1.GlobalCounter);
        Assert.Equal(initialCounter + 2, SingletonService2.GlobalCounter);

        // Get status through second reference
        var status = await SingletonService2.GetStatusAsync();
        Assert.Contains(SingletonService1.InstanceId.ToString(), status);
        Assert.Contains($"Global Counter: {initialCounter + 2}", status);
    }

    [Fact]
    public void TestSingletonServiceFromServiceProvider()
    {
        // Arrange - Get singleton service through service provider
        var singletonFromProvider = GetService<ISingletonService>();

        // Assert - Should be the same instance as the injected property
        Assert.NotNull(SingletonService1);
        Assert.NotNull(singletonFromProvider);
        Assert.Equal(SingletonService1.InstanceId, singletonFromProvider.InstanceId);
        Assert.Equal(SingletonService1.CreatedAt, singletonFromProvider.CreatedAt);
    }

    [Fact]
    public async Task TestSingletonServiceGlobalStateManagement()
    {
        // Arrange
        Assert.NotNull(SingletonService1);
        var initialCounter = SingletonService1.GlobalCounter;

        // Act - Increment multiple times
        for (int i = 0; i < 5; i++)
        {
            SingletonService1.IncrementGlobal();
        }

        // Assert - Counter should have increased by 5
        Assert.Equal(initialCounter + 5, SingletonService1.GlobalCounter);

        // Get status and verify it contains correct counter
        var status = await SingletonService1.GetStatusAsync();
        Assert.Contains($"Global Counter: {initialCounter + 5}", status);
        Assert.Contains("UTC", status);
    }

    [Fact]
    public void TestSingletonServiceWithFuncFactory()
    {
        // Arrange - Create Func<ISingletonService> factory
        var serviceFactory = GetService<Func<ISingletonService>>();
        Assert.NotNull(serviceFactory);

        // Act - Create service through factory
        var serviceThroughFactory = serviceFactory();

        // Assert - Should be the same singleton instance
        Assert.NotNull(SingletonService1);
        Assert.NotNull(serviceThroughFactory);
        Assert.Equal(SingletonService1.InstanceId, serviceThroughFactory.InstanceId);
    }

    [Fact]
    public void TestSingletonServiceWithActionDelegate()
    {
        // Arrange - Create an action delegate to work with the singleton service
        var actionInvoked = false;
        var capturedInstanceId = Guid.Empty;

        Action<ISingletonService> serviceAction = service =>
        {
            actionInvoked = true;
            capturedInstanceId = service.InstanceId;
            service.IncrementGlobal();
        };

        // Act - Execute action with the singleton service
        Assert.NotNull(SingletonService1);
        serviceAction(SingletonService1);

        // Assert - Action should have been executed with the correct singleton
        Assert.True(actionInvoked);
        Assert.Equal(SingletonService1.InstanceId, capturedInstanceId);
    }
}