using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using KiBoards.Models.Objects;
using KiBoards.Models.Settings;
using KiBoards.Models.Spaces;
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

        private string GetSpaceBaseUrl(string spaceId) => !string.IsNullOrEmpty(spaceId) ? $"/s/{spaceId.ToLower()}" : string.Empty;

        public async Task SetDarkModeAsync(bool darkMode, string spaceId, CancellationToken cancellationToken)
        {
            var content = JsonContent.Create(new KibanaSettingsRequest() { Changes = new KibanaSettingsChanges() { ThemeDarkMode = darkMode } });
            var response = await _httpClient.PostAsync($"{GetSpaceBaseUrl(spaceId)}/api/kibana/settings", content);
            response.EnsureSuccessStatusCode();
        }


        public async Task<HttpResponseMessage> SetDefaultRoute(string defaultRoute, string spaceId, CancellationToken cancellationToken)
        {
            var content = JsonContent.Create(new KibanaSettingsRequest() { Changes = new KibanaSettingsChanges() { DefaultRoute = defaultRoute } });
            return await _httpClient.PostAsync($"{GetSpaceBaseUrl(spaceId)}/api/kibana/settings", content);            
        }


        public async Task<ImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, string spaceId) => await ImportSavedObjectsAsync(ndjsonFile, spaceId, false, CancellationToken.None);
        public async Task<ImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, string spaceId, bool overwrite) => await ImportSavedObjectsAsync(ndjsonFile, spaceId, overwrite, CancellationToken.None);
        public async Task<ImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, string spaceId, bool overwrite, CancellationToken cancellationToken)
        {
            var multipartContent = new MultipartFormDataContent();
            var streamContent = new StreamContent(File.Open(ndjsonFile, FileMode.Open));
            multipartContent.Add(streamContent, "file", ndjsonFile);

            var response = await _httpClient.PostAsync($"{GetSpaceBaseUrl(spaceId)}/api/saved_objects/_import?overwrite={overwrite.ToString().ToLower()}", multipartContent);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ImportObjectsResponse>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }, cancellationToken);

            return result;
        }


        public async Task<KibanaStatusResponse> GetStatus() => await GetStatus(CancellationToken.None);
        public async Task<KibanaStatusResponse> GetStatus(CancellationToken cancellationToken) => await _httpClient.GetFromJsonAsync<KibanaStatusResponse>("api/status", cancellationToken);


        public async Task<HttpResponseMessage> TryCreateSpaceAsync(Space space, CancellationToken cancellationToken = default)
            => await _httpClient.PostAsJsonAsync("api/spaces/space", space, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }, cancellationToken);

        public async Task<HttpResponseMessage> UpdateSpaceAsync(Space space, CancellationToken cancellationToken = default)
            => await _httpClient.PutAsJsonAsync($"api/spaces/space/{space.Id}", space, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }, cancellationToken);


        public async Task<Space> GetSpaceAsync(string id) => await _httpClient.GetFromJsonAsync<Space>($"api/spaces/space/{id}", new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
    }
}
