using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Logging;

namespace Xunit.Microsoft.DependencyInjection.Abstracts
{
    public abstract class TestBedFixture : IDisposable, IAsyncDisposable
	{
		private readonly IServiceCollection _services;
		private IServiceProvider _serviceProvider;
		private bool _disposedValue;
		private bool _disposedAsync;

		protected TestBedFixture()
		{
			_services = new ServiceCollection();
			Configuration = GetConfigurationRoot();
			AddServices(_services, Configuration);
		}

		public IConfigurationRoot Configuration { get; private set; }

		public IServiceProvider GetServiceProvider(ITestOutputHelper testOutputHelper)
		{
			if (_serviceProvider != default)
			{
				return _serviceProvider;
			}

			_services.AddLogging(loggingBuilder => AddLoggingProvider(loggingBuilder, new OutputLoggerProvider(testOutputHelper)));
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

		protected abstract string GetConfigurationFile();
		protected abstract void AddServices(IServiceCollection services, IConfiguration configuration);

		protected virtual IEnumerable<string> GetConfigurationFiles()
        {
			yield return GetConfigurationFile();
        }

		protected virtual ILoggingBuilder AddLoggingProvider(ILoggingBuilder loggingBuilder, ILoggerProvider loggerProvider)
			=> loggingBuilder.AddProvider(loggerProvider);

		private IConfigurationRoot GetConfigurationRoot()
		{
			var configurationFiles = GetConfigurationFiles();
			return
				configurationFiles.All(c => !string.IsNullOrEmpty(c))
				? GetConfigurationRoot(configurationFiles)
				: default;
		}

		private IConfigurationRoot GetConfigurationRoot(IEnumerable<string> configurationFiles)
		{
			var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
			foreach(var configurationFile in configurationFiles)
            {
				builder.AddJsonFile(configurationFile);
            }
			return builder.Build();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					((ServiceProvider)_serviceProvider)?.Dispose();
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
}
