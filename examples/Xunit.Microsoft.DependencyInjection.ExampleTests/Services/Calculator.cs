using Microsoft.Extensions.Options;

namespace Xunit.Microsoft.DependencyInjection.ExampleTests.Services
{
    public class Calculator : ICalculator
    {
        private readonly Options _option;

        public Calculator(IOptions<Options> option)
            => _option = option.Value;

        public int Add(int x, int y)
            => (x + y) * _option.Rate;
    }
}
