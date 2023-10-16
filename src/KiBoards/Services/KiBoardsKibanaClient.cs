using System.Net.Http.Json;
using System.Text.Json;
using KiBoards.Models.Objects;
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


        public async Task<ImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile) => await ImportSavedObjectsAsync(ndjsonFile, false, CancellationToken.None);
        public async Task<ImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, bool overwrite) => await ImportSavedObjectsAsync(ndjsonFile, overwrite, CancellationToken.None);
        public async Task<ImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, bool overwrite, CancellationToken cancellationToken)
        {
            var multipartContent = new MultipartFormDataContent();
            var streamContent = new StreamContent(File.Open(ndjsonFile, FileMode.Open));
            multipartContent.Add(streamContent, "file", ndjsonFile);
            var response = await _httpClient.PostAsync($"/api/saved_objects/_import?overwrite={overwrite.ToString().ToLower()}", multipartContent);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ImportObjectsResponse>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }, cancellationToken);

            return result;
        }


        public async Task<KibanaStatusResponse> GetStatus() => await GetStatus(CancellationToken.None);
        public async Task<KibanaStatusResponse> GetStatus(CancellationToken cancellationToken) => await _httpClient.GetFromJsonAsync<KibanaStatusResponse>("api/status", cancellationToken);
    }
}
