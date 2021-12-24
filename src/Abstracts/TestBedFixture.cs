namespace Xunit.Microsoft.DependencyInjection.Abstracts;

public abstract class TestBedFixture : IDisposable, IAsyncDisposable
{
	private readonly IServiceCollection _services;
	private IServiceProvider? _serviceProvider;
	private bool _disposedValue;
	private bool _disposedAsync;

	protected TestBedFixture()
	{
		_services = new ServiceCollection();
		ConfigurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
		Configuration = GetConfigurationRoot();
		AddServices(_services, Configuration);
	}

	public IConfigurationRoot? Configuration { get; private set; }
	public IConfigurationBuilder ConfigurationBuilder { get; private set; }

	public IServiceProvider GetServiceProvider(ITestOutputHelper testOutputHelper)
	{
		if (_serviceProvider != default)
		{
			return _serviceProvider;
		}

		_services.AddLogging(loggingBuilder => AddLoggingProvider(loggingBuilder, new OutputLoggerProvider(testOutputHelper)));
		return _serviceProvider = _services.BuildServiceProvider();
	}

	public T? GetScopedService<T>(ITestOutputHelper testOutputHelper)
	{
		var serviceProvider = GetServiceProvider(testOutputHelper);
		using var scope = serviceProvider.CreateScope();
		return scope.ServiceProvider.GetService<T>();
	}

	public T? GetService<T>(ITestOutputHelper testOutputHelper)
		=> GetServiceProvider(testOutputHelper).GetService<T>();

	[Obsolete("GetConfigurationFile() method is now deprecated. Please override GetConfigurationFiles() method instead and yield return your current settings file name e.g. 'appsettings.json'.", true)]
	protected virtual string GetConfigurationFile() => throw new NotSupportedException();
	protected abstract void AddServices(IServiceCollection services, IConfiguration? configuration);
	protected abstract IEnumerable<string> GetConfigurationFiles();

	protected virtual ILoggingBuilder AddLoggingProvider(ILoggingBuilder loggingBuilder, ILoggerProvider loggerProvider)
		=> loggingBuilder.AddProvider(loggerProvider);

	private IConfigurationRoot? GetConfigurationRoot()
	{
		var configurationFiles = GetConfigurationFiles();
		return
			configurationFiles.All(c => !string.IsNullOrEmpty(c))
			? GetConfigurationRoot(configurationFiles)
			: default;
	}

	private IConfigurationRoot GetConfigurationRoot(IEnumerable<string> configurationFiles)
	{
		foreach (var configurationFile in configurationFiles)
		{
			ConfigurationBuilder.AddJsonFile(configurationFile);
		}
		return ConfigurationBuilder.Build();
	}

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

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			_disposedValue = true;
		}
	}

	// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	// ~AbstractDependencyInjectionFixture()
	// {
	//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public async ValueTask DisposeAsync()
	{
		if (!_disposedAsync)
		{
			await DisposeAsyncCore();
			GC.SuppressFinalize(this);
			_disposedAsync = true;
		}
	}

	protected abstract ValueTask DisposeAsyncCore();
}
