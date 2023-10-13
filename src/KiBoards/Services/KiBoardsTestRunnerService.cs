using KiBoards.Models;
using System.Reflection;
using System.Xml;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Services
{
    internal class KiBoardsTestRunnerService : IKiBoardsTestRunnerService, IDisposable
    {
        private readonly IMessageSink _messageSink;
        private readonly IKiBoardsElasticService _elasticService;

        public object Context { get; private set; }

        public KiBoardsTestRunnerService(IMessageSink messageSink, IKiBoardsElasticService elasticService)
        {
            _messageSink = messageSink;
            _elasticService = elasticService;

            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            messageSink.OnMessage(new DiagnosticMessage($"KiBoards: {version}"));
            messageSink.OnMessage(new DiagnosticMessage($"RunId: {TestFramework.RunIdentifier}"));
        }


        public void Dispose()
        {
            _messageSink.OnMessage(new DiagnosticMessage("KiBoards run finished."));
        }

        public async Task BeginTestCasesRunAsync(IEnumerable<IXunitTestCase> testCases)
        {            
            //foreach (var testCase in testCases)
            //    _messageSink.OnMessage(new DiagnosticMessage($"Discovered: {testCase.UniqueID} {testCase.DisplayName}"));

            await _elasticService.IndexTestCasesStatusAsync(testCases.ToKiBoardsTestCases(KiBoardsTestCaseStatus.Discovered, KiBoardsTestCaseState.Active, Context));            
        }


        public async Task ErrorTestCaseAsync(IXunitTestCase testCase, ITestMethod testMethod, Exception ex)
        {
            //_messageSink.OnMessage(new DiagnosticMessage($"Fatal: {testCase.UniqueID} ({ex.Message})"));
            await Task.CompletedTask;
        }

        public async Task FinishTestCaseAsync(IXunitTestCase testCase, ITestMethod testMethod, ExceptionAggregator exceptionAggregator, RunSummary summary)
        {                     
           //_messageSink.OnMessage(new DiagnosticMessage($"{(summary.Failed > 0 ? "Failure" : summary.Skipped > 0 ? "Skipped" : "Success")}: {testCase.UniqueID} {testCase.DisplayName} ({summary.Time}s)"));
            await _elasticService.IndexTestCaseStatusAsync(testCase.ToKiBoardsTestCase(testMethod, summary.ToKiBoardsTestCaseStatus(), KiBoardsTestCaseState.Inactive, Context));
        }

        public async Task StartTestCaseAsync(IXunitTestCase testCase, ITestMethod testMethod)
        {            
            //_messageSink.OnMessage(new DiagnosticMessage($"Started: {testCase.UniqueID} {testCase.DisplayName}"));
            await _elasticService.IndexTestCaseStatusAsync(testCase.ToKiBoardsTestCase(testMethod, KiBoardsTestCaseStatus.Running, KiBoardsTestCaseState.Active, Context));
        }


        public async Task EndTestCasesRunAsync(RunSummary results)
        {
            //_messageSink.OnMessage(new DiagnosticMessage("KiBoards run complete."));
            await Task.CompletedTask;

        }

        public async Task ErrorTestCasesRunAsync(IEnumerable<IXunitTestCase> testCases, Exception ex)
        {
            //_messageSink.OnMessage(new DiagnosticMessage($"Fatal: Run {RunId} failed. ({ex.Message})"));
            await Task.CompletedTask;
        }

        public void SetContext(ITestContextMessage testContext)
        {
            Context = testContext.Context;
        }

        public async Task IndexTestCaseRunAsync(ITestResultMessage testResult)
        {
            await _elasticService.IndexTestCaseRunAsync(new KiBoardsTestCaseRun()
            {
                RunIdentifier = TestFramework.RunIdentifier,
                ExecutionTime = testResult.ExecutionTime,
                Output = testResult.Output,
                Failed = testResult is ITestFailed failed ? new KiBoardsTestCaseRunFailed()
                {
                    StackTraces = failed.StackTraces,
                    ExceptionTypes = failed.ExceptionTypes,
                    Messages = failed.Messages,
                } : null,                   
                TestCase = new KiBoardsTestCase()
                {
                    DisplayName = testResult.TestCase.DisplayName,
                    UniqueId = testResult.TestCase.UniqueID,
                    SourceInformation = testResult.TestCase.SourceInformation,
                    Traits = testResult.TestCase.Traits,
                    Method = new KiBoardsTestCaseMethod()
                    {
                        Name = testResult.TestMethod.Method.Name,
                    },
                    Class = new KiBoardsTestCaseClass()
                    {
                        Name = testResult.TestClass.Class.Name,
                        Assembly = new KiBoardsTestCaseAssembly() {
                            Name = testResult.TestClass.Class.Assembly.Name,                            
                            AssemblyPath = testResult.TestClass.Class.Assembly.AssemblyPath
                        }
                    },
                    Collection = new KiBoardsTestCaseCollection()
                    {
                        DisplayName = testResult.TestClass.TestCollection.DisplayName,
                        UniqueId = testResult.TestClass.TestCollection.UniqueID,
                    }
                    },
                Skipped = testResult is ITestSkipped skipped ? new KiBoardsTestCaseRunSkipped() { Reason = skipped.Reason } : null,
                Status = testResult is ITestPassed ? "Passed" : testResult is ITestFailed ? "Failed" : testResult is ITestSkipped ? "Skipped" : "Other"
            }); 
        }
    }
}
