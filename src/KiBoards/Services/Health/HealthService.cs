using System.Reflection;

namespace KiBoards.Services
{
    public class HealthService : IHealthService
    {
        private readonly IConfiguration _configuration;

        public HealthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HealthInfo> GetHealthInfoAsync()
        {
            var info = new HealthInfo
            {
                Service = new HealthDetails()
                {
                    Name = _configuration.GetValue("APPLICATION_NAME", Assembly.GetExecutingAssembly().GetName().Name),
                    Version = _configuration.GetValue("APPLICATION_VERSION", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion)
                }
            };

            return await Task.FromResult(info);
        }
    }
}
