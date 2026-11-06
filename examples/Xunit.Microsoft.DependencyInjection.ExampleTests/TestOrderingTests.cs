using System.Reflection;
using Xunit.Microsoft.DependencyInjection.TestsOrder;
using Xunit.Sdk;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests;

/// <summary>
/// <see cref="TestPriorityOrderer"/> runs tests by <see cref="TestOrderAttribute"/> priority, then by name.
/// The method names below sort alphabetically in the opposite order to their priorities, so the
/// recorded sequence only comes out right if the priorities were honoured.
/// </summary>
[TestMethodOrderer(typeof(TestPriorityOrderer))]
public class TestOrderingTests
{
    private static readonly List<string> s_executed = [];

    [Fact, TestOrder(3)]
    public void A_RunsThird()
    {
        s_executed.Add(nameof(A_RunsThird));
        Assert.Equal([nameof(C_RunsFirst), nameof(B_RunsSecond), nameof(A_RunsThird)], s_executed);
    }

    [Fact, TestOrder(2)]
    public void B_RunsSecond() => s_executed.Add(nameof(B_RunsSecond));

    [Fact, TestOrder(1)]
    public void C_RunsFirst() => s_executed.Add(nameof(C_RunsFirst));

    /// <summary>
    /// A test case whose method cannot be located (here: a test case from another framework that
    /// carries no method information) gets the default priority instead of failing the whole run.
    /// </summary>
    [Fact]
    public void OrdererToleratesTestCasesWithoutMethodInformation()
    {
        var testCase = DispatchProxy.Create<ITestCase, NullReturningProxy>();

        var ordered = new TestPriorityOrderer().OrderTestCases<ITestCase>([testCase]);

        Assert.Same(testCase, Assert.Single(ordered));
    }

    /// <summary>Implements any interface by returning the default value from every member.</summary>
    public class NullReturningProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
            => targetMethod?.ReturnType is { IsValueType: true } valueType && valueType != typeof(void)
                ? Activator.CreateInstance(valueType)
                : null;
    }
}
