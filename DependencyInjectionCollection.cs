using System;
using Umplify.Test.Tools.Abstracts;
using Xunit;

namespace Umplify.Test.Tools
{
	[CollectionDefinition("Dependency Injection")]
	public class DependencyInjectionCollection : ICollectionFixture<TestBedFixture>
	{
		public DependencyInjectionCollection()
		{
		}
	}
}
