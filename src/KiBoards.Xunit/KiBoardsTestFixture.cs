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
    public class KiBoardsTestFixture
    {
        private readonly TestBuilderFixture _builder;
        private ITestOutputHelper? _output;

        public KiBoardsTestFixture() 
        {
            _builder = new TestBuilderFixture();
            _builder.Configuration
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
        }

        internal void SetOutputHelper(ITestOutputHelper output)
        {
            _output = output;
        }

        public IServiceCollection AddServices() => _builder.Services;


        public void Run<T>(Action<IServiceProvider, ILogger<T>> runTest, [CallerMemberName] string memberName = "")  
        {
            var config = _builder.Configuration.Build();

            var elasticOptions = new ElasticsearchSinkOptions(config.GetValue<Uri>("ELASTICSEARCH_URI"))
            {
                IndexFormat = Regex.Replace($"{typeof(T)}-kilogs-{Environment.MachineName}-{DateTime.UtcNow:yyyy-MM}".ToLower(), "[\\\\/\\*\\?\"<>\\|#., ]", "-"),
                AutoRegisterTemplate = true,
                ModifyConnectionSettings = _output == null ? null : config => config.OnRequestCompleted(d => _output?.WriteLine(d.DebugInformation))
            };
            
            _builder.Services.AddLogging(builder => builder.AddSerilog(new LoggerConfiguration()
                .WriteTo.Elasticsearch(elasticOptions)
                .Enrich.WithProperty("MemberName", memberName)
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
                logger.LogError(ex, $"Test failed.");
                throw;
            }
        }
    }
}
