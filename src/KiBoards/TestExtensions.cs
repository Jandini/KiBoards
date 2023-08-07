using KiBoards.Framework;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards
{
    public static class TestExtensions
    {
        public static ITestCase GetTestCase(this ITestOutputHelper output)
            => (output.GetType().GetField("test", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(output) as ITest).TestCase;

        public static IMessageBus GetMessageBus(this ITestOutputHelper output) =>
                output.GetType()
                .GetField("messageBus", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(output) as IMessageBus;

        public static void SetContext(this ITestOutputHelper output, object context) =>
            GetMessageBus(output).QueueMessage(new TestContextMessage(context));

    }
}
