using Polly;
using Polly.Extensions.Http;

namespace KiBoards.Services
{
    public static class KibanaExtensions
    {
        public static IServiceCollection AddKibana(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection
                .AddTransient<KibanaDelegatingHandler>()
                .AddHttpClient<IKibanaClientService, KibanaClientService>()
                  .AddPolicyHandler((services, request) => HttpPolicyExtensions
                  .HandleTransientHttpError()
                  .WaitAndRetryAsync(16,
                      retryAttempt => TimeSpan.FromSeconds(5),
                      (outcome, timespan, retryAttempt, context) =>
                      {
                          services.GetRequiredService<ILogger<IKibanaClientService>>()
                              .LogWarning("Request {uri} failed with {reason}, retry {retryAttempt} of {maxRetry} in {seconds} seconds",
                              request.RequestUri, outcome.Exception.Message,
                              retryAttempt, 16, timespan.Seconds);
                      }))
                  
                    .ConfigureHttpClient(c => c.BaseAddress = configuration.GetValue<Uri>("Kibana:Uri"))
                    .AddHttpMessageHandler<KibanaDelegatingHandler>();

            return serviceCollection.AddHostedService<KibanaHostedService>();
        }
    }
}
