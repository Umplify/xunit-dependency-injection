using System.Diagnostics.CodeAnalysis;

namespace Xunit.Microsoft.DependencyInjection.Abstracts;

/// <summary>
/// Base fixture abstraction that configures dependency injection, configuration sources
/// (JSON, user secrets, environment variables) and logging for test classes.
/// Derived fixtures register services via <see cref="AddServices"/> and configuration files
/// via <see cref="GetTestAppSettings"/>.
/// </summary>
public abstract class TestBedFixture : IDisposable, IAsyncDisposable
{
	private readonly ServiceCollection _services;
	private ServiceProvider? _serviceProvider;
	private bool _disposedValue;
	private bool _disposedAsync;
	private bool _servicesAdded;

	/// <summary>
	/// Initializes the fixture, creating a service collection and configuration builder.
	/// </summary>
	protected TestBedFixture()
	{
		_services = new ServiceCollection();
		ConfigurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
		_servicesAdded = false;
	}

	/// <summary>
	/// The combined configuration root composed of JSON files, user secrets (optional) and environment variables.
	/// </summary>
	public IConfigurationRoot? Configuration { get; private set; }

	/// <summary>
	/// The configuration builder used to assemble the <see cref="Configuration"/>.
	/// </summary>
	public IConfigurationBuilder ConfigurationBuilder { get; private set; }

	/// <summary>
	/// Builds (lazily) and returns the root <see cref="ServiceProvider"/> including logging provider and options.
	/// Subsequent calls return a cached provider.
	/// </summary>
	/// <param name="testOutputHelper">The test output helper used for logging.</param>
	public ServiceProvider GetServiceProvider(ITestOutputHelper testOutputHelper)
	{
		if (_serviceProvider is not null)
		{
			return _serviceProvider;
		}
		if (!_servicesAdded)
		{
			AddUserSecrets(ConfigurationBuilder);
			Configuration = GetConfigurationRoot();
			AddServices(_services, Configuration);
			_services.AddLogging(loggingBuilder => AddLoggingProvider(loggingBuilder, new OutputLoggerProvider(testOutputHelper)));
			_services.AddOptions();
			_servicesAdded = true;
		}
		return _serviceProvider = _services.BuildServiceProvider();
	}

	/// <summary>
	/// Resolves a scoped service of type <typeparamref name="T"/> using a new scope.
	/// </summary>
	public T? GetScopedService<T>(ITestOutputHelper testOutputHelper)
	{
		var serviceProvider = GetServiceProvider(testOutputHelper);
		using var scope = serviceProvider.CreateScope();
		return scope.ServiceProvider.GetService<T>();
	}

	/// <summary>
	/// Creates and returns a new asynchronous service scope.
	/// Caller is responsible for disposing the returned <see cref="AsyncServiceScope"/>.
	/// </summary>
	public AsyncServiceScope GetAsyncScope(ITestOutputHelper testOutputHelper)
	{
		var serviceProvider = GetServiceProvider(testOutputHelper);
		return serviceProvider.CreateAsyncScope();
	}

	/// <summary>
	/// Resolves a service of type <typeparamref name="T"/> from the root provider.
	/// </summary>
	public T? GetService<T>(ITestOutputHelper testOutputHelper)
		=> GetServiceProvider(testOutputHelper).GetService<T>();

	/// <summary>
	/// Resolves a keyed service of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="key">The key identifying the registration.</param>
	/// <param name="testOutputHelper">The test output helper used for logging and provider access.</param>
	public T? GetKeyedService<T>([DisallowNull] string key, ITestOutputHelper testOutputHelper)
		=> GetServiceProvider(testOutputHelper).GetKeyedService<T>(key);

	// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	// ~AbstractDependencyInjectionFixture()
	// {
	//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	//     Dispose(disposing: false);
	// }

	/// <inheritdoc />
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (!_disposedAsync)
		{
			await DisposeAsyncCore();
			Dispose();
			_disposedAsync = true;
		}
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Adds services to the service collection. Called once before building the provider.
	/// </summary>
	protected abstract void AddServices(IServiceCollection services, IConfiguration? configuration);

	/// <summary>
	/// Returns the test application settings descriptors (JSON files) to include.
	/// </summary>
	protected abstract IEnumerable<TestAppSettings> GetTestAppSettings();

	/// <summary>
	/// Override to asynchronously clean up resources created by the fixture.
	/// </summary>
	protected abstract ValueTask DisposeAsyncCore();

	/// <summary>
	/// Allows derived fixtures to customize logging by adding or decorating providers.
	/// </summary>
	protected virtual ILoggingBuilder AddLoggingProvider(ILoggingBuilder loggingBuilder, ILoggerProvider loggerProvider)
		=> loggingBuilder.AddProvider(loggerProvider);

	/// <summary>
	/// Override to add user secrets to the provided configuration builder when needed.
	/// Default implementation does nothing.
	/// </summary>
	protected virtual void AddUserSecrets(IConfigurationBuilder configurationBuilder) { }

	private IConfigurationRoot? GetConfigurationRoot()
	{
		var testAppSettings = GetTestAppSettings();
		return
			testAppSettings.All(setting => !string.IsNullOrEmpty(setting.Filename))
			? GetConfigurationRoot(testAppSettings)
			: default;
	}

	private IConfigurationRoot GetConfigurationRoot(IEnumerable<TestAppSettings> configurationFiles)
	{
		foreach (var configurationFile in configurationFiles)
		{
			ConfigurationBuilder.AddJsonFile(configurationFile.Filename!, optional: configurationFile.IsOptional);
		}
		ConfigurationBuilder.AddEnvironmentVariables();
		return ConfigurationBuilder.Build();
	}

	/// <summary>
	/// Disposes managed resources created by the fixture including the root service provider.
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
				if (_serviceProvider is not null)
				{
					((ServiceProvider)_serviceProvider).Dispose();
				}
				_services.Clear();
			}

			_disposedValue = true;
		}
	}
}
