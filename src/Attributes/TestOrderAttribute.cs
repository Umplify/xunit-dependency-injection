namespace Xunit.Microsoft.DependencyInjection.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestOrderAttribute(int priority) : Attribute
{
	public int Priority { get; } = priority;
}
