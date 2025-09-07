using System.Reflection;

namespace Xunit.Microsoft.DependencyInjection.Abstracts;

/// <summary>
/// Base class for test fixtures that support creating test instances with constructor injection
/// </summary>
public abstract class TestBedFactoryFixture : TestBedFixture
{
	/// <summary>
	/// Creates an instance of type T with constructor dependency injection
	/// </summary>
	/// <typeparam name="T">The type to create</typeparam>
	/// <param name="testOutputHelper">The test output helper</param>
	/// <param name="additionalParameters">Additional parameters to pass to the constructor</param>
	/// <returns>An instance of T with dependencies injected</returns>
	public T CreateTestInstance<T>(ITestOutputHelper testOutputHelper, params object[] additionalParameters)
		where T : class
	{
		var serviceProvider = GetServiceProvider(testOutputHelper);
		return CreateInstance<T>(serviceProvider, testOutputHelper, additionalParameters);
	}

	/// <summary>
	/// Creates an instance of the specified type with constructor dependency injection
	/// </summary>
	/// <param name="testType">The type to create</param>
	/// <param name="testOutputHelper">The test output helper</param>
	/// <param name="additionalParameters">Additional parameters to pass to the constructor</param>
	/// <returns>An instance of the specified type with dependencies injected</returns>
	public object CreateTestInstance(Type testType, ITestOutputHelper testOutputHelper, params object[] additionalParameters)
	{
		var serviceProvider = GetServiceProvider(testOutputHelper);
		return CreateInstance(testType, serviceProvider, testOutputHelper, additionalParameters);
	}



	/// <summary>
	/// Creates an instance of <typeparamref name="T"/> using the best matching constructor.
	/// </summary>
	private T CreateInstance<T>(IServiceProvider serviceProvider, ITestOutputHelper testOutputHelper, params object[] additionalParameters)
		where T : class => (T)CreateInstance(typeof(T), serviceProvider, testOutputHelper, additionalParameters);

	/// <summary>
	/// Core implementation for creating the test instance by iterating available constructors
	/// (public &amp; non-public) and resolving parameters from provided instances, test output helper,
	/// the fixture itself or the service provider (including keyed services when annotated).
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when no suitable constructor can be satisfied.</exception>
	private object CreateInstance(Type testType, IServiceProvider serviceProvider, ITestOutputHelper testOutputHelper, params object[] additionalParameters)
	{
		// Find the best constructor (including internal constructors)
		var constructors = testType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.OrderByDescending(c => c.GetParameters().Length)
			.ToArray();

		foreach (var constructor in constructors)
		{
			var parameters = constructor.GetParameters();
			var args = new List<object?>();
			var canResolveAll = true;

			foreach (var parameter in parameters)
			{
				object? arg = null;

				// Check if this parameter is provided in additionalParameters
				var additionalParam = additionalParameters.FirstOrDefault(p => p?.GetType().IsAssignableTo(parameter.ParameterType) == true);
				if (additionalParam != null)
				{
					arg = additionalParam;
				}
				// Special handling for ITestOutputHelper
				else if (parameter.ParameterType.IsAssignableFrom(typeof(ITestOutputHelper)))
				{
					arg = testOutputHelper;
				}
				// Special handling for the fixture itself
				else if (parameter.ParameterType.IsAssignableFrom(GetType()))
				{
					arg = this;
				}
				// Try to resolve from DI container
				else
				{
					try
					{
						// Check for keyed service attributes using a broader approach
						var allAttributes = parameter.GetCustomAttributes(true);
						object? keyValue = null;

						foreach (var attr in allAttributes)
						{
							var attrType = attr.GetType();
							// Look for any attribute with "FromKeyed" in its name and a Key property
							if (attrType.Name.Contains("FromKeyed") || attrType.FullName?.Contains("FromKeyedService") == true)
							{
								var keyProperty = attrType.GetProperty("Key");
								if (keyProperty != null)
								{
									keyValue = keyProperty.GetValue(attr);
									break;
								}
							}
						}

						if (keyValue != null && keyValue is string key)
						{
							// Use the fixture's existing GetKeyedService method instead of reflection
							try
							{
								var getKeyedServiceMethod = GetType().BaseType?.GetMethod("GetKeyedService",[typeof(string), typeof(ITestOutputHelper)])?.MakeGenericMethod(parameter.ParameterType);
								arg = getKeyedServiceMethod?.Invoke(this, [key, testOutputHelper]);
							}
							catch
							{
								// Fallback to direct reflection approach
								var method = typeof(ServiceProviderKeyedServiceExtensions)
									.GetMethod(nameof(ServiceProviderKeyedServiceExtensions.GetKeyedService), [typeof(IServiceProvider), typeof(object)])
									?.MakeGenericMethod(parameter.ParameterType);
								arg = method?.Invoke(null, [serviceProvider, keyValue]);
							}
						}
						else
						{
							arg = serviceProvider.GetService(parameter.ParameterType);
						}
					}
					catch
					{
						arg = null;
					}
				}

				// If required parameter can't be resolved, try next constructor
				if (arg == null && !parameter.HasDefaultValue)
				{
					canResolveAll = false;
					break;
				}

				args.Add(arg ?? parameter.DefaultValue);
			}

			if (canResolveAll)
			{
				try
				{
					// Use constructor.Invoke for better control over internal constructors
					return constructor.Invoke([.. args])!;
				}
				catch
				{
					// Try next constructor
					continue;
				}
			}
		}

		throw new InvalidOperationException($"Unable to create instance of {testType.Name}. No suitable constructor found or required dependencies could not be resolved.");
	}
}