using System;

namespace Xunit.Microsoft.DependencyInjection.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class TestOrderAttribute : Attribute
	{
		public TestOrderAttribute(int priority)
			=> Priority = priority;

		public int Priority { get; }
	}
}
