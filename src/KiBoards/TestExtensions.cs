using System.Reflection;
using Xunit.Abstractions;

namespace KiBoards
{
    public static class TestExtensions
    {
        public static ITestCase GetTestCase(this ITestOutputHelper output)
            => (output.GetType().GetField("test", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(output) as ITest).TestCase;
    }
}
