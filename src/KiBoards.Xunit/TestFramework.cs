using System.Reflection;
using KiBoards.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards
{
    public class TestFramework : XunitTestFramework, IDisposable
    {
        public static readonly TestRun TestRun = new();

        private readonly ServiceProvider _serviceProvider;

        public TestFramework(IMessageSink messageSink)
            : base(messageSink)
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection
                .AddSingleton(messageSink)
                .AddElasticServices()
                .AddSingleton<IKiBoardsTestRunnerService, KiBoardsTestRunnerService>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new TestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink, _serviceProvider.GetRequiredService<IKiBoardsTestRunnerService>());
        }

        public new async void Dispose()
        {
            await Task.Delay(1);
            _serviceProvider.Dispose();
            base.Dispose();
        }


        private class TestFrameworkExecutor : XunitTestFrameworkExecutor
        {
            private readonly IKiBoardsTestRunnerService _testRunner;
            private readonly IMessageSink _diagnosticMessageSink;

            public TestFrameworkExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink, IKiBoardsTestRunnerService testRunner)
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
                    TestRun.Summary = await assemblyRunner.RunAsync();
                    await _testRunner.IndexTestRunAsync(TestRun);

                }
                catch (Exception ex)
                {
                    _diagnosticMessageSink.OnMessage(new DiagnosticMessage(ex.Message, ex.StackTrace));
                }
            }
        }



        private class TestAssemblyRunner : XunitTestAssemblyRunner
        {
            private readonly IKiBoardsTestRunnerService _testRunner;

            public TestAssemblyRunner(ITestAssembly testAssembly, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions, IKiBoardsTestRunnerService testRunner)
                : base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
            {
                _testRunner = testRunner;
            }

            protected override async Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, CancellationTokenSource cancellationTokenSource)
            {
                var collectionRunner = new TestCollectionRunner(testCollection, testCases, DiagnosticMessageSink, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), cancellationTokenSource, _testRunner);
                return await collectionRunner.RunAsync();
            }

            protected override string GetTestFrameworkDisplayName()
            {
                var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                return $"KiBoards {version}";
            }         
        }


        private class TestCollectionRunner : XunitTestCollectionRunner
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


        private class TestClassRunner : XunitTestClassRunner
        {
            private readonly IKiBoardsTestRunnerService _testRunner;

            public TestClassRunner(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> collectionFixtureMappings, IKiBoardsTestRunnerService testRunner)
                : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
            {
                _testRunner = testRunner;
            }

            protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, object[] constructorArguments)
                => new TestMethodRunner(testMethod, Class, method, testCases, DiagnosticMessageSink, new TestResultBus(MessageBus), new ExceptionAggregator(Aggregator), CancellationTokenSource, constructorArguments, _testRunner)
                    .RunAsync();

            protected override async Task<RunSummary> RunTestMethodsAsync()
            {
                return await base.RunTestMethodsAsync();
            }
        }



        private class TestMethodRunner : XunitTestMethodRunner
        {
            private readonly IKiBoardsTestRunnerService _testRunner;
            private readonly TestResultBus _resultBus;
            private readonly IMessageSink _diagnosticMessageSink;

            public TestMethodRunner(ITestMethod testMethod, IReflectionTypeInfo @class, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, TestResultBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, object[] constructorArguments, IKiBoardsTestRunnerService testRunner)
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
                    _diagnosticMessageSink.OnMessage(new DiagnosticMessage(ex.Message, ex.StackTrace));
                }

                return result;
            }
        }
    }
}

