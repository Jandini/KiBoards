using Xunit.Abstractions;

namespace KiBoards
{
    internal interface ITestContextMessage : IMessageSinkMessage
    {
        public object Context { get; set; }
    }
}
