using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards
{
    internal class TestResultBus : IMessageBus
    {
        private readonly IMessageBus _messageBus;

        public ITestResultMessage TestResult { get; private set; }

        internal TestResultBus(IMessageBus messsageBus) => _messageBus = messsageBus ?? throw new ArgumentNullException(nameof(messsageBus));


        public bool QueueMessage(IMessageSinkMessage message)
        {
            if (message is ITestResultMessage result)
                TestResult = result;

            return _messageBus.QueueMessage(message);
        }

        public void Dispose()
        {
            _messageBus.Dispose();
        }
    }
}
