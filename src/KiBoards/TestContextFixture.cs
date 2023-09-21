using Xunit.Abstractions;

namespace KiBoards
{
    public class TestContextFixture 
    {
        private readonly IMessageSink _messageSink;

        public TestContextFixture(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public void SetContext(object context)
        {
            _messageSink.OnMessage(new TestContextMessage(context));
        }
    }
}
