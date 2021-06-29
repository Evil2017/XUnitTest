using System.Diagnostics;
using XUnitTest.Interfaces;

namespace XUnitTest.Implement
{
    public class Calculator : ICalculator
    {
        public decimal Addition(decimal x, decimal y)
        {
            return x + y;
        }
    }
}
