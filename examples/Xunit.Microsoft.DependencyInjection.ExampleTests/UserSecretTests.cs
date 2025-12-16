
using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

public class UserSecretTests : TestBed<TestProjectFixture>
{
    private static readonly Guid Guid = Guid.NewGuid(); // Ensure unique user secrets per test run

    private readonly string _secret1Value = $"secret1-{Guid}";
    private readonly string _secret2Value = $"secret2-{Guid}";
    
    public UserSecretTests(ITestOutputHelper testOutputHelper, TestProjectFixture fixture) : base(testOutputHelper, fixture)
    {
        SetSecret(nameof(SecretValues.Secret1), _secret1Value);
        SetSecret(nameof(SecretValues.Secret2), _secret2Value);
    }

    [Fact]
    public void TestSecretValues()
    {
        var secretValues = _fixture.GetService<IOptions<SecretValues>>(_testOutputHelper)!.Value;
        Assert.Equal(secretValues.Secret1, _secret1Value);
        Assert.Equal(secretValues.Secret2, _secret2Value);
    }

    private void SetSecret(string secretName, string secretValue)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"user-secrets set {nameof(SecretValues)}:{secretName} {secretValue}",
            WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "..", "..", "..")
        };
        var proc = Process.Start(startInfo);
        ArgumentNullException.ThrowIfNull(proc);
        
        proc.WaitForExit();
        if (proc.ExitCode != 0)
        {
            throw new Exception("Failed to set user secret");
        }
    }
}