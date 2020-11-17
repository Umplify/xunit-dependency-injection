using Xunit.Microsoft.DependencyInjection.Attributes;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests
{
    [TestCaseOrderer("Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer", "Xunit.Microsoft.DependencyInjection.TestsOrder")]
    [Collection("Dependency Injection")]
    public class UnitTests
    {
        [Fact, TestOrder(1)]
        public void Test1()
            => Assert.True(1 == 1);

        [Fact, TestOrder(2)]
        public void Test2()
            => Assert.False(1 == 0);
    }
}
