using Xunit.Abstractions;

namespace KiBoards
{
    internal class TestResultSink : IMessageSink
    {
        private IMessageSink _messageSink;

        public ITestResultMessage TestResult { get; private set; }

        internal TestResultSink(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public bool OnMessage(IMessageSinkMessage message)
        {
            if (message is ITestResultMessage result)
                TestResult = result;           

            return _messageSink.OnMessage(message);
        }
    }
}
