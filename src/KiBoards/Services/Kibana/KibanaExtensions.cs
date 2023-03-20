namespace KiBoards.Services
{
    public static class KibanaExtensions
    {
        public static IServiceCollection AddKibana(this IServiceCollection services)
        {
            return services
                .AddTransient<IKibanaClientService, KibanaClientService>()
                .AddHostedService<KibanaHostedService>();
        }
    }
}
