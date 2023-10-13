using KiBoards.Models;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace KiBoards.Services
{
    internal static class KiBoardsElasticServiceExtensions
    {
        internal static IServiceCollection AddElasticServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IElasticClient>(new ElasticClient(ConfigureIndexes(new ConnectionSettings(new Uri($"http://localhost:9200"))
                    .MaxRetryTimeout(TimeSpan.FromMinutes(5))                    
                    .EnableApiVersioningHeader() // EnableApiVersioningHeader resolves internal errors with bulk index Invalid NEST response built from a successful (200) low level call on POST: /_bulk
                    .MaximumRetries(3))))
                .AddTransient<IKiBoardsElasticService, KiBoardsElasticService>();
        }

        internal static ConnectionSettings ConfigureIndexes(ConnectionSettings connectionSettings)
        {
            connectionSettings.DefaultMappingFor<TestRun>(m => m
                .IndexName($"kiboards-testruns-{DateTime.UtcNow:yyyy-MM}")
                .IdProperty(p => p.Id));

            connectionSettings.DefaultMappingFor<KiBoardsTestCaseRun>(m => m
                .IndexName($"kiboards-testcases-{DateTime.UtcNow:yyyy-MM}"));
            
            return connectionSettings;
        }
    }
}
