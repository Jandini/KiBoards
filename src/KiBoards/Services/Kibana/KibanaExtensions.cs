using Polly;
using Polly.Extensions.Http;

namespace KiBoards.Services
{
    public static class KibanaExtensions
    {
        public static IServiceCollection AddKibana(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var retryDelay = TimeSpan.FromMilliseconds(configuration.GetValue("Kibana:Client:RequestRetryDelay", 16));
            var retryCount = configuration.GetValue("Kibana:Client:RequestRetryCount", 5000);

            serviceCollection
                .AddTransient<KibanaDelegatingHandler>()
                .AddHttpClient<IKibanaClientService, KibanaClientService>()
                  .AddPolicyHandler((services, request) => HttpPolicyExtensions
                  .HandleTransientHttpError()
                  .OrResult(result => result.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                  .WaitAndRetryAsync(retryCount,
                      retryAttempt => retryDelay,
                      (outcome, timespan, retryAttempt, context) =>
                      {
                          services.GetRequiredService<ILogger<IKibanaClientService>>()
                              .LogWarning("Request {uri} failed with {reason}, retry {retryAttempt} of {maxRetry} in {ms}ms",
                              request.RequestUri, outcome.Exception.Message,
                              retryAttempt, retryCount, timespan.Seconds);
                      }))
                  
                    .ConfigureHttpClient(c => c.BaseAddress = configuration.GetValue<Uri>("KIBANA_URI"))
                    .AddHttpMessageHandler<KibanaDelegatingHandler>();

            return serviceCollection.AddHostedService<KibanaHostedService>();
        }
    }
}
