namespace Xunit.Microsoft.DependencyInjection.Attributes;

/// <summary>
/// Specifies execution ordering for test methods when used with <see cref="TestsOrder.TestPriorityOrderer"/>.
/// Lower priority values execute first; methods with the same priority are ordered alphabetically.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestOrderAttribute(int priority) : Attribute
{
	/// <summary>
	/// Gets the priority assigned to the test method.
	/// </summary>
	public int Priority { get; } = priority;
}
