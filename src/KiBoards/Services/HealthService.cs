using System.Reflection;

namespace KiBoards.Services
{
    public class HealthService : IHealthService
    {
        public async Task<HealthInfo> GetHealthInfoAsync(HttpRequest request)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            var info = new HealthInfo
            {
                ServiceName = assembly.GetName().Name,
                ServiceVersion = version,
                ServiceHost = request.Host.ToString()
            };

            return await Task.FromResult(info);
        }
    }
}
