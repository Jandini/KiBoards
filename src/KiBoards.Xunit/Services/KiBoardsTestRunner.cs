using KiBoards.Models;
using Nest;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Services
{
    internal class KiBoardsTestRunner
    {
        private readonly KiBoardsElasticClient _elasticService;

        public KiBoardsTestRunner(IMessageSink messageSink)
        {
            var uriString = Environment.GetEnvironmentVariable("KIB_ELASTICSEARCH_HOST") ?? "http://localhost:9200";
            var connectionSettings = new ConnectionSettings(new Uri(uriString));

            var elasticClient = new ElasticClient(connectionSettings
                .DefaultMappingFor<TestRun>(m => m                    
                    .IndexName($"kiboards-testruns-{DateTime.UtcNow:yyyy-MM}")
                    .IdProperty(p => p.Id))
                .DefaultMappingFor<KiBoardsTestCaseRun>(m => m                    
                    .IndexName($"kiboards-testcases-{DateTime.UtcNow:yyyy-MM}"))
                    
                .MaxRetryTimeout(TimeSpan.FromMinutes(5))
                .EnableApiVersioningHeader() 
                .MaximumRetries(3));
            

            _elasticService = new KiBoardsElasticClient(elasticClient, messageSink);

            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            messageSink.OnMessage(new DiagnosticMessage($"KiBoards.Xunit {version} logging to {uriString}"));
        }

        public async Task IndexTestRunAsync(TestRun testRun)
        {
            await _elasticService.IndexDocumentAsync(testRun);
        }

        public async Task IndexTestCaseRunAsync(ITestResultMessage testResult)
        {
            await _elasticService.IndexDocumentAsync(new KiBoardsTestCaseRun()
            {
                Id = (TestFramework.TestRun.Id + testResult.TestCase.UniqueID).ComputeMD5(),
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
