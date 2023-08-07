using Xunit.Abstractions;
using Xunit.Sdk;
using KiBoards.Services;

namespace KiBoards.Framework
{
    internal class TestCollectionRunner : XunitTestCollectionRunner
    {
        private readonly IKiBoardsTestRunnerService _testRunner;

        public TestCollectionRunner(ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IKiBoardsTestRunnerService testRunner)
            : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            _testRunner = testRunner;
        }

        protected override Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
            => new TestClassRunner(testClass, @class, testCases, DiagnosticMessageSink, MessageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), CancellationTokenSource, CollectionFixtureMappings, _testRunner)
                .RunAsync();
    }
}
