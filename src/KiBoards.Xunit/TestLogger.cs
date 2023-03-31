using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Serilog.Sinks.Elasticsearch;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KiBoards.Xunit.Logger
{
    [FriendlyName("kiboards")]
    [ExtensionUri("logger://Microsoft/TestPlatform/XunitKiBoardsLogger/v1")]
    public class TestLogger : ITestLoggerWithParameters
    {

        static Guid _runGuid = Guid.NewGuid();

        static TestLogger()
        {
            Console.WriteLine($"Logging with KiBoards {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
        }

        #region ITestLoggerWithParameters
        public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters) => Initialize(events);
        public void Initialize(TestLoggerEvents events, string testRunDirectory) => Initialize(events);
        #endregion


        public void Initialize(TestLoggerEvents events)
        {
            // Create elasticserach logging options
            var elasticOptions = new ElasticsearchSinkOptions()
            {
                // Ensure index name meet the following criteria https://www.elastic.co/guide/en/elasticsearch/reference/current/indices-create-index.html
                IndexFormat = Regex.Replace($"kiboards-logs-{Environment.UserName}-{DateTime.UtcNow:yyyy-MM}".ToLower(), "[\\\\/\\*\\?\"<>\\|#., ]", "-"),
                AutoRegisterTemplate = true,
            };

            // Elasticsearch index name must not be longer than 255 characters
            if (elasticOptions.IndexFormat.Length > 255)
                throw new Exception("Elasticsearch index name exceeds 255 characters.");

            // Create serilog logger
            var logger = new LoggerConfiguration()
                .WriteTo.Elasticsearch(elasticOptions)
                .Enrich.WithMachineName()
                .Enrich.WithProperty("RunGuid", _runGuid)
                .CreateLogger();             

            events.TestResult += (sender, e) =>
            {
                Console.WriteLine($"-> {e.Result.DisplayName}\t{e.Result.Duration}\t{e.Result.Outcome}");
                
                logger.Information("{@TestResult}", new
                {
                    e.Result.TestCase.Id,
                    TestCaseDisplayName = e.Result.TestCase.DisplayName,
                    e.Result.TestCase.Source,
                    e.Result.Outcome,
                    e.Result.DisplayName,
                    e.Result.Duration,
                    e.Result.ErrorMessage,
                    e.Result.StartTime,
                    e.Result.EndTime,
                });
            };

            events.TestRunComplete += (sender, e) =>
            {
                logger.Information("{@TestRunComplete}", new
                {
                    e.ElapsedTimeInRunningTests,
                    e.IsAborted,
                    e.IsCanceled,
                    e.Error
                });

                // Ensure to flush serilog logger
                logger.Dispose();                
            };
        }
    }
}
