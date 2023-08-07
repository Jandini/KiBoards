using Xunit.Abstractions;

namespace KiBoards.Framework
{
    internal class TestContextMessage : IMessageSinkMessage, IExecutionMessage
    {
        public object Context { get; set; }

        public IEnumerable<ITestCase> TestCases => throw new NotImplementedException();

        public TestContextMessage(object context)
        {
            Context = context;
        }
    }
}
