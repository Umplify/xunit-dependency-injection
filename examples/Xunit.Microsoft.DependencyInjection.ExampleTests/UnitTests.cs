using System.Threading.Tasks;
using Xunit.Microsoft.DependencyInjection.Attributes;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests
{
    [TestCaseOrderer("Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer", "Xunit.Microsoft.DependencyInjection")]
    public class UnitTests
    {
        [Fact, TestOrder(1)]
        public void Test1()
            => Assert.True(1 == 1);

        [Fact, TestOrder(2)]
        public void Test2()
            => Assert.False(1 == 0);

        [Fact, TestOrder(3)]
        public async Task Test3()
        {
            await Task.Delay(3000);
            Assert.True(1 == 1);
        }

        [Fact, TestOrder(4)]
        public async Task Test4()
        {
            await Task.Delay(5000);
            Assert.True(1 > 0);
        }
    }
}
