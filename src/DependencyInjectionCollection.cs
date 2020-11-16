using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Xunit.Microsoft.DependencyInjection
{
    [CollectionDefinition("Dependency Injection")]
	public class DependencyInjectionCollection : ICollectionFixture<TestBedFixture>
	{
		public DependencyInjectionCollection()
		{
		}
	}
}
