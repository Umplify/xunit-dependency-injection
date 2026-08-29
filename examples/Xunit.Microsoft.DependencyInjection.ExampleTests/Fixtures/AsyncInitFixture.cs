namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Fixtures;

/// <summary>
/// Options carrying a value that only exists after asynchronous initialization completes,
/// mimicking a connection string handed out by a container or remote resource.
/// </summary>
public sealed record AsyncInitOptions(string ConnectionString);

/// <summary>
/// Demonstrates asynchronous fixture initialization. <see cref="TestBedFixture"/> implements
/// xUnit.net's <c>IAsyncLifetime</c>, so overriding <see cref="TestBedFixture.InitializeAsyncCore"/>
/// runs async setup after construction and before the first test resolves anything — the values it
/// produces can therefore feed the registrations made in <see cref="AddServices"/>.
/// </summary>
public class AsyncInitFixture : TestBedFixture
{
	private string? _connectionString;
	private int _initializationCount;

	public int InitializationCount => _initializationCount;

	protected override async ValueTask InitializeAsyncCore()
	{
		Interlocked.Increment(ref _initializationCount);
		// Stands in for genuinely asynchronous work: starting a Testcontainer,
		// seeding a database, fetching configuration from a remote source, etc.
		await Task.Yield();
		_connectionString = "Server=initialized-async";
	}

	protected override void AddServices(IServiceCollection services, IConfiguration configuration)
		=> services.AddSingleton(new AsyncInitOptions(_connectionString
			?? throw new InvalidOperationException("InitializeAsyncCore must complete before AddServices runs.")));

	// Note: no DisposeAsyncCore override — it is virtual with a no-op default.
}
