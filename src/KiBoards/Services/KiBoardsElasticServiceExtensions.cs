using KiBoards.Models;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

namespace KiBoards.Services
{
    internal static class KiBoardsElasticServiceExtensions
    {
        internal static IServiceCollection AddElasticServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IElasticClient>(new ElasticClient(ConfigureIndexes(new ConnectionSettings(new Uri($"http://localhost:9200"))
                    .MaxRetryTimeout(TimeSpan.FromMinutes(5))
                    .MaximumRetries(3))))
                .AddTransient<IKiBoardsElasticService, KiBoardsElasticService>();
        }

        internal static ConnectionSettings ConfigureIndexes(ConnectionSettings connectionSettings)
        {
            connectionSettings.DefaultMappingFor<KiBoardsTestCaseStatusDto>(m => m
                .IndexName($"kiboards-testcase-status-{DateTime.UtcNow:yyyy-MM}")
                .IdProperty(p => p.UniqueId));

            return connectionSettings;
        }
    }
}
