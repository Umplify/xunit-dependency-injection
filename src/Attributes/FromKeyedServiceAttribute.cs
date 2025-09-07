namespace Xunit.Microsoft.DependencyInjection.Attributes;

/// <summary>
/// Attribute to mark constructor parameters that should be resolved as keyed services in factory injection
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class FromKeyedServiceAttribute : Attribute
{
	/// <summary>
	/// Gets the key for the keyed service
	/// </summary>
	public string Key { get; }

	/// <summary>
	/// Initializes a new instance of the FromKeyedServiceAttribute
	/// </summary>
	/// <param name="key">The key for the keyed service</param>
	public FromKeyedServiceAttribute(string key) => Key = key ?? throw new ArgumentNullException(nameof(key));
}