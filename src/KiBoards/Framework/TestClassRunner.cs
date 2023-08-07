using Xunit.Abstractions;
using Xunit.Sdk;
using KiBoards.Services;

namespace KiBoards.Framework
{
    internal class TestClassRunner : XunitTestClassRunner
    {
        private readonly IKiBoardsTestRunnerService _testRunner;

        public TestClassRunner(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> collectionFixtureMappings, IKiBoardsTestRunnerService testRunner)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
        {
            _testRunner = testRunner;
        }

        protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, object[] constructorArguments)
            => new TestMethodRunner(testMethod, Class, method, testCases, DiagnosticMessageSink, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource, constructorArguments, _testRunner)
                .RunAsync();

        protected override async Task<RunSummary> RunTestMethodsAsync()
        {
            return await base.RunTestMethodsAsync();
        }
    }
}
