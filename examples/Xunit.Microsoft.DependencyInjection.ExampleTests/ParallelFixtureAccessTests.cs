using Microsoft.Extensions.DependencyInjection;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// xUnit.net v4 can run every test in an assembly in parallel (<c>ParallelMode.All</c>), which means
/// several tests sharing a single <see cref="Abstracts.TestBedFixture"/> may resolve services from it
/// at the same time. These tests pin down that the fixture builds exactly one container no matter how
/// many threads reach it first.
/// </summary>
public class ParallelFixtureAccessTests
{
	[Fact]
	public void GetServiceProvider_ConcurrentFirstAccess_BuildsSingleProvider()
	{
		using var fixture = new Fixtures.TestProjectFixture();
		var outputHelper = TestContext.Current.TestOutputHelper!;

		var providers = new ServiceProvider[64];
		Parallel.For(0, providers.Length, i => providers[i] = fixture.GetServiceProvider(outputHelper));

		Assert.Single(providers.Distinct());
	}

	[Fact]
	public void GetService_ConcurrentFirstAccess_ResolvesTheSameSingleton()
	{
		using var fixture = new Fixtures.TestProjectFixture();
		var outputHelper = TestContext.Current.TestOutputHelper!;

		var singletons = new ISingletonService?[64];
		Parallel.For(0, singletons.Length, i => singletons[i] = fixture.GetService<ISingletonService>(outputHelper));

		Assert.All(singletons, singleton => Assert.NotNull(singleton));
		Assert.Single(singletons.Distinct());
	}
}
