using KiBoards.Models;
using Nest;
using System.Collections;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Services
{
    internal class KiBoardsTestRunner
    {
        private readonly KiBoardsElasticClient _elasticService;
        private readonly KiBoardsTestRun _testRun;

        public string Version {  get; private set; }

        public KiBoardsTestRunner(IMessageSink messageSink)
        {
            try
            {
                Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

                _testRun = new KiBoardsTestRun()
                {
                    Id = Guid.NewGuid().ToString(),
                    StartTime = DateTime.UtcNow,
                    MachineName = Environment.MachineName,
                    UserName = Environment.UserName,
                    FrameworkVersion = Version,
                    Variables = new Dictionary<string, string>()
                };

                var startupAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetCustomAttribute<KiboardsTestStartupAttribute>() != null).ToArray();
                foreach (var assembly in startupAssemblies)
                    Startup(assembly, messageSink);

                foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
                {
                    var name = entry.Key.ToString();
                    var value = entry.Value.ToString();

                    const string prefix = "KIB_VAR_";

                    if (name.Length > prefix.Length + 1 && name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value))
                        _testRun.Variables.TryAdd(name[prefix.Length..], value);
                }

                var uriString = Environment.GetEnvironmentVariable("KIB_ELASTICSEARCH_HOST") ?? "http://localhost:9200";
                var connectionSettings = new ConnectionSettings(new Uri(uriString));
                
                var elasticClient = new ElasticClient(connectionSettings
                    .DefaultMappingFor<KiBoardsTestRun>(m => m
                        .IndexName($"kiboards-testruns-{DateTime.UtcNow:yyyy-MM}")
                        .IdProperty(p => p.Id))
                    .DefaultMappingFor<KiBoardsTestCaseRun>(m => m
                        .IndexName($"kiboards-testcases-{DateTime.UtcNow:yyyy-MM}"))

                    .MaxRetryTimeout(TimeSpan.FromMinutes(5))
                    .EnableApiVersioningHeader()
                    .MaximumRetries(3));

                _elasticService = new KiBoardsElasticClient(elasticClient, messageSink);
                
                messageSink.WriteMessage($"KiBoards.Xunit {Version} logging to {uriString}");
            }
            catch (Exception ex)
            {
                messageSink.WriteException(ex);
            }
        }


        private void Startup(Assembly assembly, IMessageSink messageSink)
        {
            try
            {                               
                var startup = assembly.GetCustomAttribute<KiboardsTestStartupAttribute>();
                Type type = assembly.GetType(startup.ClassName);

                if (type != null)
                {
                    messageSink.WriteMessage($"Invoking {type.FullName}");

                    if (type.GetConstructor(new Type[] { typeof(string), typeof(IMessageSink) }) != null)
                        Activator.CreateInstance(type, _testRun.Id, messageSink);
                    else if (type.GetConstructor(new Type[] { typeof(string) }) != null)
                        Activator.CreateInstance(type, _testRun.Id);
                    else if (type.GetConstructor(new Type[] { typeof(IMessageSink) }) != null)
                        Activator.CreateInstance(type, messageSink);
                    else if (type.GetConstructor(new Type[] { }) != null)
                        Activator.CreateInstance(type);
                }
            }
            catch (Exception ex)
            {
                messageSink.WriteException(ex);
            }
        }

        public async Task IndexTestRunAsync(RunSummary summary)
        {
            _testRun.Summary = new KiBoardsTestRunSummary()
            {
                Total = summary.Total,
                Failed = summary.Failed,
                Skipped = summary.Skipped,
                Time = summary.Time,
            };

            _testRun.Status = summary.Failed > 0 ? "Failed" : summary.Skipped == summary.Total ? "Skipped" : "Passed";

            await _elasticService.IndexDocumentAsync(_testRun);
        }

        public async Task IndexTestCaseRunAsync(ITestResultMessage testResult)
        {
            await _elasticService.IndexDocumentAsync(new KiBoardsTestCaseRun()
            {
                Id = (_testRun.Id + testResult.TestCase.UniqueID).ComputeMD5(),
                TestRun = _testRun,
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

        internal void UpdateRun(IEnumerable<IXunitTestCase> testCases)
        {
            _testRun.Name = string.Join(",", testCases.Select(a => Path.GetFileNameWithoutExtension(a.TestMethod.TestClass.Class.Assembly.AssemblyPath)).Distinct());
            _testRun.Hash = string.Join(",", testCases.OrderBy(a => a.UniqueID).Select(a => a.UniqueID)).ComputeMD5();
        }
    }
}
