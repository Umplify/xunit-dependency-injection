using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Umplify.Test.Tools.Logging;
using Xunit.Abstractions;

namespace Umplify.Test.Tools.Abstracts
{
	public abstract class TestBedFixture : IDisposable
	{
		private readonly IServiceCollection _services;
		private IServiceProvider _serviceProvider;
		private bool _disposedValue;

		protected TestBedFixture()
		{
			_services = new ServiceCollection();
			AddServices(_services, GetConfigurationRoot());
		}

		public IServiceProvider GetServiceProvider(ITestOutputHelper testOutputHelper)
		{
			if (_serviceProvider != default)
			{
				return _serviceProvider;
			}

			_services.AddLogging(loggingBuilder => loggingBuilder.AddProvider(new OutputLoggerProvider(testOutputHelper)));
			return _serviceProvider = _services.BuildServiceProvider();
		}

		public T GetScopedService<T>(ITestOutputHelper testOutputHelper)
		{
			var serviceProvider = GetServiceProvider(testOutputHelper);
			using var scope = serviceProvider.CreateScope();
			return scope.ServiceProvider.GetService<T>();
		}

		public T GetService<T>(ITestOutputHelper testOutputHelper)
			=> GetServiceProvider(testOutputHelper).GetService<T>();

		public void AddOutputHelperToLoggerProvider(ITestOutputHelper testOutputHelper)
			=> _services.AddScoped<ILoggerProvider>(_ => new OutputLoggerProvider(testOutputHelper));

		protected abstract string GetConfigurationFile();
		protected abstract void AddServices(IServiceCollection services, IConfiguration configuration);

		private IConfigurationRoot GetConfigurationRoot()
		{
			var configurationFile = GetConfigurationFile();

			return
				!string.IsNullOrEmpty(configurationFile)
				? GetConfigurationRoot(configurationFile)
				: default;
		}

		private IConfigurationRoot GetConfigurationRoot(string configurationFile) =>
			new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile(configurationFile)
			.Build();

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					((ServiceProvider)_serviceProvider)?.Dispose();
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
	}
}
