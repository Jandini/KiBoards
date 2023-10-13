using KiBoards.Models;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Services
{
    internal class KiBoardsTestRunnerService : IKiBoardsTestRunnerService
    {
        private readonly IKiBoardsElasticService _elasticService;

        public KiBoardsTestRunnerService(IMessageSink messageSink, IKiBoardsElasticService elasticService)
        {            
            _elasticService = elasticService;

            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            messageSink.OnMessage(new DiagnosticMessage($"Running KiBoards.xUnit: {version}"));
        }

        public async Task IndexTestRunAsync(TestRun testRun)
        {
            await _elasticService.IndexDocumentAsync(testRun);
        }

        public async Task IndexTestCaseRunAsync(ITestResultMessage testResult)
        {
            await _elasticService.IndexDocumentAsync(new KiBoardsTestCaseRun()
            {
                TestRun = TestFramework.TestRun,
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
