using System.Net.Http.Json;
using KiBoards.Models.Settings;
using KiBoards.Models.Status;

namespace KiBoards.Services
{
    internal class KiBoardsKibanaClient 
    {
        private readonly HttpClient _httpClient;        

        public KiBoardsKibanaClient(Uri kibanaUri, HttpClient httpClinet)
        {
            _httpClient = httpClinet;    
            _httpClient.BaseAddress = kibanaUri;
            _httpClient.DefaultRequestHeaders.Add("kbn-xsrf", "true");
        }

        public async Task SetDarkModeAsync(bool darkMode, CancellationToken cancellationToken)
        {
            var content = JsonContent.Create(new KibanaSettingsRequest() { Changes = new KibanaSettingsChanges() { ThemeDarkMode = darkMode } });
            var response = await _httpClient.PostAsync("api/kibana/settings", content);
            response.EnsureSuccessStatusCode();
        }


        public async Task<KibanaStatusResponse> GetStatus(CancellationToken cancellationToken) => await _httpClient.GetFromJsonAsync<KibanaStatusResponse>("api/status", cancellationToken);



        public async Task WaitForKibanaAsync(CancellationToken cancellationToken, int retryDelay = 5000)
        {
            while (!cancellationToken.IsCancellationRequested) 
            {
                try
                {
                    var response = await GetStatus(cancellationToken);
                    
                    string level = response?.Status?.Overall?.Level ?? throw new Exception("Kibana status is not available.");

                    if (level != "available")
                        throw new Exception("Kibana not available.");
                    
                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(retryDelay, cancellationToken);
                }
            }
        }
    }
}
