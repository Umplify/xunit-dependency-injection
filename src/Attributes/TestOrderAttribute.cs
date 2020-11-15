using System;

namespace Umplify.Test.Tools.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class TestOrderAttribute : Attribute
	{
		public TestOrderAttribute(int priority)
			=> Priority = priority;

		public int Priority { get; }
	}
}
