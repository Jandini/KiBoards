namespace KiBoards.Services
{
    public static class HealthExtensions
    {
        public static IServiceCollection AddHealth(this IServiceCollection services)
        {
            return services.AddScoped<IHealthService, HealthService>();
        }
    }
}
