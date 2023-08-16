using Xunit.Abstractions;

namespace KiBoards.Framework
{
    internal class TestContextMessage : ITestContextMessage
    {
        public object Context { get; set; }

        public TestContextMessage(object context)
        {
            Context = context;
        }
    }
}
