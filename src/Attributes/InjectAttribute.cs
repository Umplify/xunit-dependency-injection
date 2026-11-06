namespace Xunit.Microsoft.DependencyInjection.Attributes;

/// <summary>
/// Attribute to mark properties for dependency injection in TestBedWithDI classes
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class InjectAttribute : Attribute
{
	/// <summary>
	/// Gets the optional key for keyed services
	/// </summary>
	public string? Key { get; init; }

	/// <summary>
	/// When <see langword="true"/>, constructing the test class fails with an
	/// <see cref="InvalidOperationException"/> naming the property, the service type and the fixture
	/// if the service cannot be resolved. When <see langword="false"/> (the default) the property is
	/// left <see langword="null"/>.
	/// </summary>
	public bool Required { get; init; }

	/// <summary>
	/// Initializes a new instance of the InjectAttribute
	/// </summary>
	public InjectAttribute() { }

	/// <summary>
	/// Initializes a new instance of the InjectAttribute with a key for keyed services
	/// </summary>
	/// <param name="key">The key for keyed services</param>
	public InjectAttribute(string key) => Key = key;
}