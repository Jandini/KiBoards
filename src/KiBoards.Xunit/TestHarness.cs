using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace KiBoards.Xunit
{
    public class TestHarness
    {
        private readonly TestBuilder _builder;
        private ITestOutputHelper? _output;

        public TestHarness() 
        {
            _builder = new TestBuilder();
            _builder.Configuration
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
        }

        public void SetOutputHelper(ITestOutputHelper output)
        {
            _output = output;
        }

        public IServiceCollection AddServices() => _builder.Services;


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="runTest"></param>
        /// <param name="method">Test method name. Leave empty to generate it.</param>
        public void Run<T>(Action<IServiceProvider, ILogger<T>> runTest, [CallerMemberName] string method = "")  
        {
            var config = _builder.Configuration.Build();

            var elasticOptions = new ElasticsearchSinkOptions(config.GetValue<Uri>("ELASTICSEARCH_URI"))
            {
                IndexFormat = Regex.Replace($"{typeof(T)}-logs-{Environment.MachineName}-{DateTime.UtcNow:yyyy-MM}".ToLower(), "[\\\\/\\*\\?\"<>\\|#., ]", "-"),
                AutoRegisterTemplate = true,
                ModifyConnectionSettings = _output == null ? null : config => config.OnRequestCompleted(d => _output?.WriteLine(d.DebugInformation)),
            };
            
            _builder.Services.AddLogging(builder => builder.AddSerilog(new LoggerConfiguration()
                .WriteTo.Elasticsearch(options: elasticOptions)
                .Enrich.WithProperty("ApplicationName", typeof(T).Assembly.GetName().Name)
                .Enrich.WithProperty("Method", method)
                .Enrich.WithMachineName()
                .CreateLogger(), true));

            var provider = _builder.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<T>>();

            try
            {
                runTest(provider, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{typeof(T)}.{method} failed");  
                throw;
            }
        }
    }
}
