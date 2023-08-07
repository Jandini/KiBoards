using Xunit.Abstractions;
using Xunit.Sdk;
using KiBoards.Services;
using System.Reflection;

namespace KiBoards.Framework
{
    internal class TestAssemblyRunner : XunitTestAssemblyRunner
    {
        private readonly IKiBoardsTestRunnerService _testRunner;
        private readonly IMessageSink _messageSink;        

        public TestAssemblyRunner(ITestAssembly testAssembly, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions, IKiBoardsTestRunnerService testRunner)
            : base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
        {
            _testRunner = testRunner;
            _messageSink = diagnosticMessageSink;
        }

        protected override async Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, CancellationTokenSource cancellationTokenSource)
        {        
            var collectionRunner = new TestCollectionRunner(testCollection, testCases, DiagnosticMessageSink, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), cancellationTokenSource, _testRunner);
            return await collectionRunner.RunAsync();
        }

        protected override IMessageBus CreateMessageBus()
        {
            _messageSink.OnMessage(new DiagnosticMessage("Creating MessageBus"));
            return new TestMessageBus(base.CreateMessageBus(), _messageSink);
        }

        protected override string GetTestFrameworkDisplayName()
        {
            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            return $"KiBoards {version}";
        }

        protected override Task BeforeTestAssemblyFinishedAsync()
        {
            _messageSink.OnMessage(new DiagnosticMessage("BeforeTestAssemblyFinishedAsync"));
            return base.BeforeTestAssemblyFinishedAsync();
        }
    }
}
