using Xunit.Abstractions;

namespace KiBoards.Framework
{
    internal interface ITestContextMessage : IMessageSinkMessage
    {
        public object Context { get; set; }
    }
}
