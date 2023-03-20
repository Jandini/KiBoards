using System.Text;

namespace KiBoards.Services
{
    public class KibanaClientService : IKibanaClientService
    {
        private readonly ILogger<KibanaClientService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;        

        public KibanaClientService(ILogger<KibanaClientService> logger, IConfiguration configuration, HttpClient client)
        {
            _logger = logger;
            _configuration = configuration;
            _client = client;            
            _client.BaseAddress = configuration.GetValue<Uri>("Kibana:Uri");

        }

        public async Task SetDarkModeAsync(CancellationToken cancellationToken)
        {
            var darkMode = _configuration.GetValue("Kibana:Theme:DarkMode", false).ToString().ToLower();
            _logger.LogInformation("Setting theme:darkMode={darkMode}", darkMode);
            var content = new StringContent($"{{\"changes\":{{\"theme:darkMode\":{darkMode}}}}}", Encoding.UTF8, "application/json");
            content.Headers.Add("kbn-xsrf", "true");
            await _client.PostAsync("api/kibana/settings", content, cancellationToken);
        }

        public async Task WaitForKibanaAsync(CancellationToken cancellationToken)
        {
            var level = "unknown";
            _logger.LogInformation("Waiting for Kibana {uri}", _client.BaseAddress);

            while (!cancellationToken.IsCancellationRequested) 
            {
                try
                {
                    var response = await _client.GetFromJsonAsync<KibanaStatusResponse>("api/status", cancellationToken);
                    level = response?.Status?.Overall?.Level ?? throw new Exception("Kibana status is not available.");

                    if (level != "available")
                        throw new Exception($"Kibana status is {level}.");

                    _logger.LogInformation("Kibana name: {name}", response.Name);
                    _logger.LogInformation("Kibana version: {version}", response.Version?.Number);
                    _logger.LogInformation("Kibana status level: {status}", level);
                    _logger.LogInformation("Kibana status summary: {status}", response.Status.Overall.Summary);

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Kibana status is {level}", level);
                    
                    var delay = _configuration.GetValue("Kibana:RetryDelayMs", 10000);
                    _logger.LogInformation("Retry in {ms} milliseconds", delay);

                    await Task.Delay(delay, cancellationToken);
                }
            }
        }
    }
}
