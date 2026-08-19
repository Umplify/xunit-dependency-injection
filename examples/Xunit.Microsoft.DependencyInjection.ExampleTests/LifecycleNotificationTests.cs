namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Example showing that a <c>TestBedFixture</c> implementing xUnit.net v4's
/// <c>INotifyTestClassLifecycleAsync</c> is notified when the consuming test class starts,
/// which gives fixtures a place to do per-class setup alongside their DI registrations.
/// </summary>
public class LifecycleNotificationTests : TestBedWithDI<LifecycleAwareFixture>
{
	private readonly LifecycleAwareFixture _lifecycleFixture;

	public LifecycleNotificationTests(ITestOutputHelper testOutputHelper, LifecycleAwareFixture fixture)
		: base(testOutputHelper, fixture) => _lifecycleFixture = fixture;

	[Inject]
	private ICalculator? Calculator { get; set; }

	[Fact]
	public void FixtureIsNotifiedThatTheTestClassStarted()
		=> Assert.Contains(nameof(LifecycleNotificationTests), _lifecycleFixture.StartedClasses);

	[Fact]
	public async Task FixtureStillResolvesServicesNormally()
	{
		Assert.NotNull(Calculator);
		Assert.Equal(40, await Calculator.AddAsync(2, 2));
	}
}
