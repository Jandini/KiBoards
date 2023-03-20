namespace KiBoards.Services
{
    public class KibanaHostedService : IHostedService
    {
        private readonly ILogger<KibanaHostedService> _logger;
        private readonly IKibanaClientService _kibanaClient;
        private readonly IConfiguration _configuration;


        public KibanaHostedService(ILogger<KibanaHostedService> logger, IKibanaClientService kibanaClient, IConfiguration configuration)
        {
            _logger = logger;
            _kibanaClient = kibanaClient;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Kibana hosted service");

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await _kibanaClient.WaitForKibanaAsync(cancellationToken);
                    await _kibanaClient.SetDarkModeAsync(cancellationToken);
                }
                catch (TaskCanceledException tce)
                {
                    _logger.LogWarning(tce.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kibana hosted service failed.");
                }


            }, cancellationToken);

            _logger.LogInformation("Kibana hosted service started");

            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
