using System.Reflection;
using Xunit.v3;

namespace Xunit.Microsoft.DependencyInjection.TestsOrder;

/// <summary>
/// Orders tests by the value specified in <see cref="Attributes.TestOrderAttribute"/>.
/// Tests with equal priority are then ordered alphabetically by method name for determinism.
/// </summary>
/// <remarks>
/// xUnit.net v4 orders the methods of a class and the test cases of a method separately.
/// To run the methods of a class in priority order, apply this type as a method orderer:
/// <c>[TestMethodOrderer(typeof(TestPriorityOrderer))]</c>. Applied only as a case orderer
/// (<c>[TestCaseOrderer(typeof(TestPriorityOrderer))]</c>) it orders the cases within each method,
/// which does not change the order in which the methods run.
/// </remarks>
public class TestPriorityOrderer : ITestCaseOrderer, ITestMethodOrderer
{
	/// <summary>
	/// Orders the provided test cases based on <see cref="Attributes.TestOrderAttribute"/>.
	/// </summary>
	public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
		where TTestCase : ITestCase
		=> testCases
			.OrderBy(testCase => GetPriority(testCase is IXunitTestCase xunitTestCase ? xunitTestCase.TestMethod : null))
			.ThenBy(testCase => testCase.TestMethod?.MethodName, StringComparer.OrdinalIgnoreCase)
			.ToList()
			.AsReadOnly();

	/// <summary>
	/// Orders the test methods of a class based on <see cref="Attributes.TestOrderAttribute"/>.
	/// </summary>
	public IReadOnlyCollection<TTestMethod?> OrderTestMethods<TTestMethod>(IReadOnlyCollection<TTestMethod?> testMethods)
		where TTestMethod : ITestMethod
		=> testMethods
			.OrderBy(testMethod => GetPriority(testMethod as IXunitTestMethod))
			.ThenBy(testMethod => testMethod?.MethodName, StringComparer.OrdinalIgnoreCase)
			.ToList()
			.AsReadOnly();

	/// <summary>
	/// Reads the <see cref="TestOrderAttribute"/> priority from the method xUnit.net already resolved.
	/// Tests without one (for example ones produced by a different test framework) get the default priority of 0.
	/// </summary>
	private static int GetPriority(IXunitTestMethod? testMethod)
		=> testMethod?.Method?.GetCustomAttribute<TestOrderAttribute>()?.Priority ?? 0;
}
