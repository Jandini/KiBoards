using System.Net.Http.Json;

namespace KiBoards.Services
{
    internal class KibanaClient 
    {
        private readonly HttpClient _httpClient;        

        public KibanaClient(HttpClient httpClinet, Uri kibanaUri)
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

        public async Task WaitForKibanaAsync(CancellationToken cancellationToken)
        {
            var delay = 5000;

            while (!cancellationToken.IsCancellationRequested) 
            {
                try
                {                    
                    var response = await _httpClient.GetFromJsonAsync<KibanaStatusResponse>("api/status", cancellationToken);
                    string level = response?.Status?.Overall?.Level ?? throw new Exception("Kibana status is not available.");

                    if (level != "available")
                        throw new Exception("Kibana not available.");
                    
                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }
    }
}
