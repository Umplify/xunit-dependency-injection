using Options = Xunit.Microsoft.DependencyInjection.ExampleTests.Services.Options;
using Xunit.v3;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

/// <summary>
/// Demonstrates xUnit.net v4's fixture lifecycle notifications. A <see cref="TestBedFixture"/>
/// can implement <see cref="INotifyTestClassLifecycleAsync"/> to run setup and teardown around the
/// test class that consumes it, without an extra fixture type or a custom test framework.
/// </summary>
public class LifecycleAwareFixture : TestBedFixture, INotifyTestClassLifecycleAsync
{
	private readonly List<string> _startedClasses = [];
	private readonly List<string> _finishedClasses = [];

	public IReadOnlyList<string> StartedClasses => _startedClasses;

	public IReadOnlyList<string> FinishedClasses => _finishedClasses;

	public ValueTask OnTestClassStartingAsync(IXunitTestClass testClass)
	{
		_startedClasses.Add(testClass.TestClassSimpleName);
		return default;
	}

	public ValueTask OnTestClassFinishedAsync(IXunitTestClass testClass)
	{
		_finishedClasses.Add(testClass.TestClassSimpleName);
		return default;
	}

	protected override void AddServices(IServiceCollection services, IConfiguration configuration)
		=> services
			.AddSingleton<ICalculator, Calculator>()
			.Configure<Options>(config => configuration.GetSection("Options").Bind(config));

	protected override IEnumerable<TestAppSettings> GetTestAppSettings()
	{
		yield return new() { Filename = "appsettings.json", IsOptional = false };
	}

	protected override ValueTask DisposeAsyncCore() => new();
}
