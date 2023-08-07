using KiBoards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Services
{
    internal class KiBoardsTestRunnerService : IKiBoardsTestRunnerService, IDisposable
    {
        private readonly IMessageSink _messageSink;
        private readonly IKiBoardsElasticService _elasticService;

        public Guid RunId { get; private set; } = Guid.NewGuid();

        public KiBoardsTestRunnerService(IMessageSink messageSink, IKiBoardsElasticService elasticService)
        {
            _messageSink = messageSink;
            _elasticService = elasticService;

            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            messageSink.OnMessage(new DiagnosticMessage($"KiBoards: {version}"));
            messageSink.OnMessage(new DiagnosticMessage($"RunId: {RunId}"));
        }


        public void Dispose()
        {
            //_messageSink.OnMessage(new DiagnosticMessage("KiBoards run finished."));
        }

        public async Task BeginTestCasesRunAsync(IEnumerable<IXunitTestCase> testCases)
        {            
            //foreach (var testCase in testCases)
            //    _messageSink.OnMessage(new DiagnosticMessage($"Discovered: {testCase.UniqueID} {testCase.DisplayName}"));

            await _elasticService.IndexTestCasesAsync(testCases.ToKiBoardsTestCases(KiBoardsTestCaseStatus.Discovered, KiBoardsTestCaseState.Active));            
        }


        public async Task ErrorTestCaseAsync(IXunitTestCase testCase, ITestMethod testMethod, Exception ex)
        {
            //_messageSink.OnMessage(new DiagnosticMessage($"Fatal: {testCase.UniqueID} ({ex.Message})"));
            await Task.CompletedTask;
        }

        public async Task FinishTestCaseAsync(IXunitTestCase testCase, ITestMethod testMethod, ExceptionAggregator exceptionAggregator, RunSummary summary)
        {                     
           //_messageSink.OnMessage(new DiagnosticMessage($"{(summary.Failed > 0 ? "Failure" : summary.Skipped > 0 ? "Skipped" : "Success")}: {testCase.UniqueID} {testCase.DisplayName} ({summary.Time}s)"));
            await _elasticService.IndexTestCaseAsync(testCase.ToKiBoardsTestCase(testMethod, summary.ToKiBoardsTestCaseStatus(), KiBoardsTestCaseState.Inactive));            
        }

        public async Task StartTestCaseAsync(IXunitTestCase testCase, ITestMethod testMethod)
        {            
            //_messageSink.OnMessage(new DiagnosticMessage($"Started: {testCase.UniqueID} {testCase.DisplayName}"));
            await _elasticService.IndexTestCaseAsync(testCase.ToKiBoardsTestCase(testMethod, KiBoardsTestCaseStatus.Running, KiBoardsTestCaseState.Active));
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
    }
}
