using System.Reflection;

namespace Xunit.Microsoft.DependencyInjection.Abstracts;

/// <summary>
/// Enhanced test bed that supports dependency injection through constructor parameters
/// while maintaining compatibility with the existing fixture-based approach.
/// </summary>
/// <typeparam name="TFixture">The fixture type derived from TestBedFixture</typeparam>
public abstract class TestBedWithDI<TFixture> : TestBed<TFixture>
    where TFixture : TestBedFixture
{
    protected TestBedWithDI(ITestOutputHelper testOutputHelper, TFixture fixture) 
        : base(testOutputHelper, fixture)
    {
        // Resolve and inject dependencies for derived classes
        InjectDependencies();
    }

    /// <summary>
    /// Injects dependencies into properties marked with [Inject] attribute or constructor parameters
    /// </summary>
    private void InjectDependencies()
    {
        var derivedType = GetType();
        
        // Inject dependencies into properties marked with [Inject] attribute
        InjectProperties(derivedType);
    }

    /// <summary>
    /// Injects dependencies into properties marked with [Inject] attribute
    /// </summary>
    private void InjectProperties(Type derivedType)
    {
        var injectableProperties = derivedType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(prop => prop.GetCustomAttribute<InjectAttribute>() != null && prop.CanWrite);

        foreach (var property in injectableProperties)
        {
            var service = ResolveService(property.PropertyType, property.GetCustomAttribute<InjectAttribute>()?.Key);
            if (service != null)
            {
                property.SetValue(this, service);
            }
        }
    }

    /// <summary>
    /// Resolves a service from the fixture's service provider
    /// </summary>
    /// <param name="serviceType">The type of service to resolve</param>
    /// <param name="key">Optional key for keyed services</param>
    /// <returns>The resolved service instance or null</returns>
    protected object? ResolveService(Type serviceType, string? key = null)
    {
        try
        {
            var serviceProvider = _fixture.GetServiceProvider(_testOutputHelper);
            
            if (!string.IsNullOrEmpty(key))
            {
                // Try to resolve keyed service
                var keyedServiceMethod = typeof(ServiceProviderKeyedServiceExtensions)
                    .GetMethod(nameof(ServiceProviderKeyedServiceExtensions.GetKeyedService), [typeof(IServiceProvider), typeof(object)])
                    ?.MakeGenericMethod(serviceType);
                    
                return keyedServiceMethod?.Invoke(null, [serviceProvider, key]);
            }
            else
            {
                // Resolve regular service
                return serviceProvider.GetService(serviceType);
            }
        }
        catch
        {
            // Return null if service cannot be resolved
            return null;
        }
    }

    /// <summary>
    /// Resolves a service of type T from the fixture's service provider
    /// </summary>
    /// <typeparam name="T">The service type</typeparam>
    /// <returns>The resolved service instance</returns>
    protected T? GetService<T>() => _fixture.GetService<T>(_testOutputHelper);

    /// <summary>
    /// Resolves a scoped service of type T from the fixture's service provider
    /// </summary>
    /// <typeparam name="T">The service type</typeparam>
    /// <returns>The resolved service instance</returns>
    protected T? GetScopedService<T>() => _fixture.GetScopedService<T>(_testOutputHelper);

    /// <summary>
    /// Resolves a keyed service of type T from the fixture's service provider
    /// </summary>
    /// <typeparam name="T">The service type</typeparam>
    /// <param name="key">The service key</param>
    /// <returns>The resolved service instance</returns>
    protected T? GetKeyedService<T>(string key) => _fixture.GetKeyedService<T>(key, _testOutputHelper);
}