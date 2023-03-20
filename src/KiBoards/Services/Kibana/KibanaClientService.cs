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
        }

        public async Task SetDarkModeAsync(CancellationToken cancellationToken)
        {
            var darkMode = _configuration.GetValue("Kibana:Settings:Theme:DarkMode", false);
            _logger.LogInformation("Setting theme:darkMode={darkMode}", darkMode);
            var content = JsonContent.Create(new KibanaSettingsRequest() { Changes = new KibanaSettingsChanges() { ThemeDarkMode = darkMode } });
            var response = await _client.PostAsync("api/kibana/settings", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task WaitForKibanaAsync(CancellationToken cancellationToken)
        {
            var level = "unknown";
            var delay = TimeSpan.FromMilliseconds(_configuration.GetValue("Kibana:RetryDelayMs", 10000));

            _logger.LogInformation("Waiting for Kibana {uri}", _client.BaseAddress);

            while (!cancellationToken.IsCancellationRequested) 
            {
                try
                {                    
                    var response = await _client.GetFromJsonAsync<KibanaStatusResponse>("api/status", cancellationToken);
                    level = response?.Status?.Overall?.Level ?? throw new Exception("Kibana status is not available.");

                    if (level != "available")
                        throw new Exception("Kibana not available.");
                    
                    _logger.LogInformation("Kibana name: {name}", response.Name);
                    _logger.LogInformation("Kibana version: {version}", response.Version?.Number);
                    _logger.LogInformation("Kibana status level: {status}", level);
                    _logger.LogInformation("Kibana status summary: {status}", response.Status.Overall.Summary);

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Waiting for Kibana");
                    _logger.LogWarning("Kibana status is {level}, next retry in {seconds} seconds", level, delay.TotalSeconds);
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }
    }
}
