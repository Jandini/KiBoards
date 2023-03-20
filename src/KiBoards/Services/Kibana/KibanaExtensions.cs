namespace KiBoards.Services
{
    public static class KibanaExtensions
    {
        public static IServiceCollection AddKibana(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<KibanaDelegatingHandler>()
                .AddHttpClient<IKibanaClientService, KibanaClientService>()
                    .ConfigureHttpClient(c => c.BaseAddress = configuration.GetValue<Uri>("Kibana:Uri"))
                    .AddHttpMessageHandler<KibanaDelegatingHandler>();

            return services.AddHostedService<KibanaHostedService>();
        }
    }
}
