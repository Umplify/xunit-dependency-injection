using System.Diagnostics.CodeAnalysis;

namespace Xunit.Microsoft.DependencyInjection.Abstracts;

/// <summary>
/// Base fixture abstraction that configures dependency injection, configuration sources
/// (JSON, user secrets, environment variables) and logging for test classes.
/// Derived fixtures register services via <see cref="AddServices"/> and configuration files
/// via <see cref="GetTestAppSettings"/>.
/// </summary>
public abstract class TestBedFixture : IDisposable, IAsyncDisposable, IAsyncLifetime
{
	private readonly ServiceCollection _services;
	private readonly Lock _serviceProviderLock = new();
	private volatile ServiceProvider? _serviceProvider;
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
	/// <remarks>
	/// Initialization is thread-safe, so a fixture shared by tests that run concurrently
	/// (for example under xUnit.net v4's <c>ParallelMode.All</c>) still builds exactly one container.
	/// </remarks>
	/// <param name="testOutputHelper">The test output helper used for logging.</param>
	public ServiceProvider GetServiceProvider(ITestOutputHelper testOutputHelper)
	{
		if (_serviceProvider is not null)
		{
			return _serviceProvider;
		}

		lock (_serviceProviderLock)
		{
			if (_serviceProvider is not null)
			{
				return _serviceProvider;
			}
			if (!_servicesAdded)
			{
				Configuration = GetConfigurationRoot();
				AddServices(_services, Configuration);
				_services.AddLogging(loggingBuilder => AddLoggingProvider(loggingBuilder, new OutputLoggerProvider(testOutputHelper, followCurrentTest: true)));
				_services.AddOptions();
				_servicesAdded = true;
			}
			var serviceProviderOptions = GetServiceProviderOptions();
			return _serviceProvider = serviceProviderOptions is null
				? _services.BuildServiceProvider()
				: _services.BuildServiceProvider(serviceProviderOptions);
		}
	}

	/// <summary>
	/// Builds (lazily) and returns the root <see cref="ServiceProvider"/>, as
	/// <see cref="GetServiceProvider(ITestOutputHelper)"/> does, without requiring a test output helper.
	/// Loggers created by the container write to the output of whichever test is running when they log.
	/// </summary>
	public ServiceProvider GetServiceProvider()
		=> GetServiceProvider(TestContext.Current.TestOutputHelper ?? NullTestOutputHelper.Instance);

	/// <summary>
	/// Resolves a service of type <typeparamref name="T"/> from the root provider.
	/// </summary>
	public T? GetService<T>()
		=> GetServiceProvider().GetService<T>();

	/// <summary>
	/// Resolves a keyed service of type <typeparamref name="T"/> from the root provider.
	/// </summary>
	/// <param name="key">The key identifying the registration.</param>
	public T? GetKeyedService<T>([DisallowNull] string key)
		=> GetServiceProvider().GetKeyedService<T>(key);

	/// <summary>
	/// Creates and returns a new asynchronous service scope.
	/// Caller is responsible for disposing the returned <see cref="AsyncServiceScope"/>.
	/// </summary>
	public AsyncServiceScope GetAsyncScope()
		=> GetServiceProvider().CreateAsyncScope();

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

	/// <summary>
	/// Called by xUnit.net after the fixture is constructed and before the first test that uses it
	/// runs. Delegates to <see cref="InitializeAsyncCore"/>; override that method for async setup.
	/// </summary>
	public virtual ValueTask InitializeAsync() => InitializeAsyncCore();

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
			if (_serviceProvider is not null)
			{
				await _serviceProvider.DisposeAsync();
				_serviceProvider = null;
			}
			Dispose();
			_disposedAsync = true;
		}
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Adds services to the service collection. Called once before building the provider.
	/// </summary>
	protected abstract void AddServices(IServiceCollection services, IConfiguration configuration);

	/// <summary>
	/// Returns the test application settings descriptors (JSON files) to include.
	/// </summary>
	protected virtual IEnumerable<TestAppSettings> GetTestAppSettings() => [];

	/// <summary>
	/// Override to asynchronously prepare resources before the service container is built.
	/// Runs exactly once, after construction and before the first test resolves anything, so values
	/// produced here (connection strings, started containers) can be consumed by <see cref="AddServices"/>.
	/// The container does not exist yet, so services cannot be resolved from within this method.
	/// If initialization fails partway, release anything already created before letting the exception
	/// propagate. Default implementation does nothing.
	/// </summary>
	protected virtual ValueTask InitializeAsyncCore() => new();

	/// <summary>
	/// Override to asynchronously clean up resources created by the fixture.
	/// Default implementation does nothing.
	/// </summary>
	protected virtual ValueTask DisposeAsyncCore() => new();

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

	/// <summary>
	/// Controls the order in which configuration sources are added, and therefore which one wins
	/// when the same key appears in more than one.
	/// </summary>
	/// <remarks>
	/// <para>
	/// When <see langword="false"/> (the default in 10.x) the order is user secrets, then the JSON files
	/// from <see cref="GetTestAppSettings"/>, then environment variables, so a value in a JSON file
	/// overrides the same key in user secrets.
	/// </para>
	/// <para>
	/// Override and return <see langword="true"/> to use the standard .NET order: JSON files, then user
	/// secrets, then environment variables, so user secrets override the JSON files.
	/// The standard order is planned to become the default in version 11.0.
	/// </para>
	/// </remarks>
	protected virtual bool UseStandardConfigurationPrecedence => false;

	/// <summary>
	/// Override to supply the <see cref="ServiceProviderOptions"/> used when the container is built,
	/// for example to set <see cref="ServiceProviderOptions.ValidateScopes"/> so that resolving a scoped
	/// service from the root provider throws, or <see cref="ServiceProviderOptions.ValidateOnBuild"/> so
	/// that missing dependencies are reported when the container is built rather than on first use.
	/// </summary>
	/// <returns>
	/// The options to build the container with, or <see langword="null"/> (the default) to use the
	/// container's defaults, under which neither validation is performed.
	/// </returns>
	protected virtual ServiceProviderOptions? GetServiceProviderOptions() => null;

	private IConfigurationRoot GetConfigurationRoot()
	{
		var useStandardPrecedence = UseStandardConfigurationPrecedence;
		if (!useStandardPrecedence)
		{
			// Same hook order as before 10.2: user secrets first, then the JSON files, then environment variables.
			AddUserSecrets(ConfigurationBuilder);
		}
		foreach (var configurationFile in GetTestAppSettings().Where(appSetting => !string.IsNullOrEmpty(appSetting.Filename)))
		{
			ConfigurationBuilder.AddJsonFile(configurationFile.Filename!, optional: configurationFile.IsOptional);
		}
		if (useStandardPrecedence)
		{
			AddUserSecrets(ConfigurationBuilder);
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
