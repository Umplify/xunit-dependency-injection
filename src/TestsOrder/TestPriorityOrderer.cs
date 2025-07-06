using System.Reflection;
using Xunit.v3;

namespace Xunit.Microsoft.DependencyInjection.TestsOrder;

public class TestPriorityOrderer : ITestCaseOrderer
{
	public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
		where TTestCase : ITestCase
	{
		var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

		foreach (var testCase in testCases)
		{
			var priority = 0;
			var testMethod = testCase.TestMethod;
			var type = Type.GetType(testMethod?.TestClass.TestClassNamespace ?? string.Empty) ?? AppDomain.CurrentDomain
					.GetAssemblies()
					.Select(a => a.GetType(testMethod?.TestClass?.TestClassName ?? string.Empty))
					.FirstOrDefault(t => t != null);
			var method = type?.GetMethod(testMethod?.MethodName ?? string.Empty);
			var attributes = method?.GetCustomAttributes(typeof(TestOrderAttribute));
			foreach (var attr in attributes!)
			{
				if (attr is TestOrderAttribute orderAttr)
				{
					priority = orderAttr.Priority;
				}
			}

			GetOrCreate(sortedMethods, priority).Add(testCase);
		}

		var testCaseCollection = new List<TTestCase>();
		foreach (var list in sortedMethods.Keys.Select(priority => sortedMethods[priority]))
		{
			list.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod?.MethodName, y.TestMethod?.MethodName));
			foreach (var testCase in list)
			{
				testCaseCollection.Add(testCase);
			}
		}

		return testCaseCollection.AsReadOnly();
	}

	private TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
		where TValue : new()
	{
		if (dictionary.TryGetValue(key, out var result))
		{
			return result;
		}

		result = new TValue();
		dictionary[key] = result;

		return result;
	}
}
