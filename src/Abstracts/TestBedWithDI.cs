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
	/// <summary>
	/// Initializes the test bed and performs property injection for members marked with <see cref="InjectAttribute"/>.
	/// </summary>
	/// <param name="testOutputHelper">The xUnit test output helper.</param>
	/// <param name="fixture">The fixture instance supplying services.</param>
	protected TestBedWithDI(ITestOutputHelper testOutputHelper, TFixture fixture)
		: base(testOutputHelper, fixture) =>
		// Resolve and inject dependencies for derived classes
		InjectDependencies();

	/// <summary>
	/// Initializes the test bed with the output helper of the test that is being run, taken from
	/// <see cref="TestContext.Current"/>, and performs property injection for members marked with
	/// <see cref="InjectAttribute"/>. Derived test classes only need to accept the fixture.
	/// </summary>
	/// <param name="fixture">The fixture instance supplying services.</param>
	/// <exception cref="InvalidOperationException">
	/// No test output helper is available, because the instance is not being created by xUnit.net for a test.
	/// </exception>
	protected TestBedWithDI(TFixture fixture)
		: base(fixture) =>
		InjectDependencies();

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
			var injectAttribute = property.GetCustomAttribute<InjectAttribute>()!;
			var service = injectAttribute.Required
				? ResolveRequiredService(property, injectAttribute.Key)
				: ResolveService(property.PropertyType, injectAttribute.Key);
			if (service != null)
			{
				property.SetValue(this, service);
			}
		}
	}

	private object ResolveRequiredService(PropertyInfo property, string? key)
	{
		var target = $"Cannot inject required property {property.DeclaringType?.Name}.{property.Name}";
		var service = string.IsNullOrEmpty(key)
			? property.PropertyType.Name
			: $"{property.PropertyType.Name} with key \"{key}\"";
		object? resolved;
		try
		{
			resolved = ResolveServiceCore(property.PropertyType, key);
		}
		catch (Exception exception)
		{
			throw new InvalidOperationException(
				$"{target}: resolving {service} from {typeof(TFixture).Name} failed. {exception.Message}", exception);
		}
		return resolved ?? throw new InvalidOperationException(
			$"{target}: {service} is not registered in {typeof(TFixture).Name}.");
	}

	private object? ResolveServiceCore(Type serviceType, string? key)
	{
		var serviceProvider = _fixture.GetServiceProvider(_testOutputHelper);
		return string.IsNullOrEmpty(key)
			? serviceProvider.GetService(serviceType)
			: ((IKeyedServiceProvider)serviceProvider).GetKeyedService(serviceType, key);
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
			return ResolveServiceCore(serviceType, key);
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
	/// <summary>
	/// Convenience helper to resolve a service of type <typeparamref name="T"/>.
	/// </summary>
	protected T? GetService<T>() => _fixture.GetService<T>(_testOutputHelper);

	/// <summary>
	/// Resolves a scoped service of type T from the fixture's service provider
	/// </summary>
	/// <typeparam name="T">The service type</typeparam>
	/// <returns>The resolved service instance</returns>
	/// <summary>
	/// Convenience helper to resolve a scoped service of type <typeparamref name="T"/>.
	/// </summary>
	protected T? GetScopedService<T>() => _fixture.GetScopedService<T>(_testOutputHelper);

	/// <summary>
	/// Resolves a keyed service of type T from the fixture's service provider
	/// </summary>
	/// <typeparam name="T">The service type</typeparam>
	/// <param name="key">The service key</param>
	/// <returns>The resolved service instance</returns>
	protected T? GetKeyedService<T>(string key) => _fixture.GetKeyedService<T>(key, _testOutputHelper);
}