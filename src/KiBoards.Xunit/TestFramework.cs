using System.Reflection;
using KiBoards.Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards
{
    public class TestFramework : XunitTestFramework, IDisposable
    {        
        readonly KiBoardsTestRunner _testRunner;               

        public TestFramework(IMessageSink messageSink)
            : base(messageSink)
        {          
            _testRunner = new KiBoardsTestRunner(messageSink);
        }


        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new TestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink, _testRunner);
        }     


        private class TestFrameworkExecutor : XunitTestFrameworkExecutor
        {
            private readonly KiBoardsTestRunner _testRunner;
            private readonly IMessageSink _diagnosticMessageSink;

            public TestFrameworkExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink, KiBoardsTestRunner testRunner)
                : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
            {
                _testRunner = testRunner;
                _diagnosticMessageSink = diagnosticMessageSink;                             
            }                     

            protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
            {
                try
                {
                    using var assemblyRunner = new TestAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions, _testRunner);
                    var summary = await assemblyRunner.RunAsync();                    
                    await _testRunner.IndexTestRunAsync(summary);

                }
                catch (Exception ex)
                {
                    _diagnosticMessageSink.WriteException(ex);
                }
            }
        }


        private class TestAssemblyRunner : XunitTestAssemblyRunner
        {
            private readonly KiBoardsTestRunner _testRunner;

            public TestAssemblyRunner(ITestAssembly testAssembly, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions, KiBoardsTestRunner testRunner)
                : base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
            {
                _testRunner = testRunner;
            }

            protected override async Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, CancellationTokenSource cancellationTokenSource)
            {
                var collectionRunner = new TestCollectionRunner(testCollection, testCases, DiagnosticMessageSink, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), cancellationTokenSource, _testRunner);
                return await collectionRunner.RunAsync();
            }

            protected override string GetTestFrameworkDisplayName() => $"KiBoards {_testRunner.Version}";
        }


        private class TestCollectionRunner : XunitTestCollectionRunner
        {
            private readonly KiBoardsTestRunner _testRunner;

            public TestCollectionRunner(ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, KiBoardsTestRunner testRunner)
                : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
            {
                _testRunner = testRunner;
            }

            protected override Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
                => new TestClassRunner(testClass, @class, testCases, DiagnosticMessageSink, MessageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), CancellationTokenSource, CollectionFixtureMappings, _testRunner)
                    .RunAsync();
        }


        private class TestClassRunner : XunitTestClassRunner
        {
            private readonly KiBoardsTestRunner _testRunner;

            public TestClassRunner(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> collectionFixtureMappings, KiBoardsTestRunner testRunner)
                : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
            {                
                _testRunner = testRunner;
                testRunner.UpdateRun(testCases);
            }

            protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, object[] constructorArguments)
                => new TestMethodRunner(testMethod, Class, method, testCases, DiagnosticMessageSink, new TestResultBus(MessageBus), new ExceptionAggregator(Aggregator), CancellationTokenSource, constructorArguments, _testRunner)
                    .RunAsync();            
        }


        private class TestResultBus : IMessageBus
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

        private class TestMethodRunner : XunitTestMethodRunner
        {
            private readonly KiBoardsTestRunner _testRunner;
            private readonly TestResultBus _resultBus;
            private readonly IMessageSink _diagnosticMessageSink;

            public TestMethodRunner(ITestMethod testMethod, IReflectionTypeInfo @class, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, TestResultBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, object[] constructorArguments, KiBoardsTestRunner testRunner)
                 : base(testMethod, @class, method, testCases, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource, constructorArguments)

            {
                _diagnosticMessageSink = diagnosticMessageSink;
                _resultBus = messageBus;
                _testRunner = testRunner;
            }

            protected override async Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
            {
                var result = await base.RunTestCaseAsync(testCase);

                try
                {
                    await _testRunner.IndexTestCaseRunAsync(_resultBus.TestResult);
                }
                catch (Exception ex)
                {
                    _diagnosticMessageSink.WriteException(ex);
                }

                return result;
            }
        }
    }
}

