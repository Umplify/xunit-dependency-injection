namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// Example showing that a <c>TestBedFixture</c> can perform asynchronous setup by overriding
/// <c>InitializeAsyncCore</c>. xUnit.net awaits it after constructing the fixture and before the
/// first test runs; the container is built lazily on first use, so values produced during
/// initialization are available to <c>AddServices</c>.
/// </summary>
public class AsyncInitTests(ITestOutputHelper testOutputHelper, AsyncInitFixture fixture)
	: TestBed<AsyncInitFixture>(testOutputHelper, fixture)
{
	[Fact]
	public void ValueProducedDuringInitializationIsRegisteredInTheContainer()
	{
		var options = _fixture.GetService<AsyncInitOptions>(_testOutputHelper);

		Assert.NotNull(options);
		Assert.Equal("Server=initialized-async", options.ConnectionString);
	}

	[Fact]
	public void FixtureIsInitializedExactlyOnce()
	{
		// Force the container to build before asserting, so this holds
		// regardless of which test in the class runs first.
		_ = _fixture.GetService<AsyncInitOptions>(_testOutputHelper);

		Assert.Equal(1, _fixture.InitializationCount);
	}

	[Fact]
	public void InitializationDoesNotRepeatForTestsSharingTheFixture()
		=> Assert.Equal(1, _fixture.InitializationCount);
}
