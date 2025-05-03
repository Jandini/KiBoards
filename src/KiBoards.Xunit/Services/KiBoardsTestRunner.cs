using KiBoards.Models;
using Nest;
using System.Collections;
using System.Reflection;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KiBoards.Services
{
    internal class KiBoardsTestRunner
    {
        private readonly KiBoardsElasticClient _elasticService;
        private readonly Func<KiBoardsTestRun> _testRunFactory;
        private readonly string _runId = Guid.NewGuid().ToString();
        private string _runName;
        private string _runHash;

        public string Version { get; private set; }

        public KiBoardsTestRunner(IMessageSink messageSink)
        {
            Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            var variables = new Dictionary<string, string>();
            var startTime = DateTime.Now;
            var userName = Environment.UserName;
            var machineName = Environment.MachineName;

            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                var name = entry.Key.ToString();
                var value = entry.Value.ToString();
               
                const string prefix = "KIB_VAR_";

                if (name.Length > prefix.Length + 1 && name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value))
                    variables.TryAdd(name[prefix.Length..], value);

                const string base64 = "KIB_B64_";

                if (name.Length > base64.Length + 1 && name.StartsWith(base64, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(value))
                {
                    try
                    {
                        value = Encoding.UTF8.GetString(Convert.FromBase64String(value));
                    }
                    catch (Exception ex) 
                    {
                        messageSink.WriteException(ex);
                    }

                    variables.TryAdd(name[base64.Length..], value);
                }
            }

            _testRunFactory = () => new KiBoardsTestRun()
            {
                RunId = _runId,
                StartTime = startTime,
                MachineName = machineName,
                UserName = userName,
                FrameworkVersion = Version,
                Variables = variables,
                Name = _runName,
                Hash = _runHash,
            };

            var startupAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetCustomAttribute<KiboardsTestStartupAttribute>() != null).ToArray();

            foreach (var assembly in startupAssemblies)
                Startup(assembly, messageSink);

            var uriString = Environment.GetEnvironmentVariable("KIB_ELASTICSEARCH_HOST") ?? "http://localhost:9200";
            var connectionSettings = new ConnectionSettings(new Uri(uriString));

            var elasticClient = new ElasticClient(connectionSettings
                .DefaultMappingFor<KiBoardsTestRun>(m => m
                    .IndexName($"kiboards-testruns-{DateTime.UtcNow:yyyy-MM}"))
                .MaxRetryTimeout(TimeSpan.FromMinutes(5))
                .EnableApiVersioningHeader()
                .MaximumRetries(3));

            _elasticService = new KiBoardsElasticClient(elasticClient, messageSink);

            messageSink.WriteMessage($"KiBoards.Xunit {Version} logging to {uriString}");
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
                        Activator.CreateInstance(type, _runId, messageSink);
                    else if (type.GetConstructor(new Type[] { typeof(string) }) != null)
                        Activator.CreateInstance(type, _runId);
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
            var testRun = _testRunFactory();

            testRun.Summary = new KiBoardsTestRunSummary()
            {
                Total = summary.Total,
                Failed = summary.Failed,
                Skipped = summary.Skipped,
                Time = summary.Time,
            };

            testRun.Status = summary.Failed > 0 ? "Failed" : summary.Skipped == summary.Total ? "Skipped" : "Passed";

          
            await _elasticService.IndexDocumentAsync(testRun);
        }

        public async Task IndexTestCaseRunAsync(DateTime startedAt, ITestResultMessage testResult)
        {
            var testRun = _testRunFactory();

            testRun.Test = new KiBoardsTestCaseRun()
            {
                Id = (testRun.RunId + testResult.TestCase.UniqueID).ComputeMD5(),
                StartedAt = startedAt,
                FinishedAt = DateTime.UtcNow,
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
                        Assembly = new KiBoardsTestCaseAssembly()
                        {
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
            };

            await _elasticService.IndexDocumentAsync(testRun);
        }

        internal void UpdateRun(IEnumerable<IXunitTestCase> testCases)
        {
            _runName = string.Join(",", testCases.Select(a => Path.GetFileNameWithoutExtension(a.TestMethod.TestClass.Class.Assembly.AssemblyPath)).Distinct());
            _runHash = string.Join(",", testCases.OrderBy(a => a.UniqueID).Select(a => a.UniqueID)).ComputeMD5();
        }
    }
}
