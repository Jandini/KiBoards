using KiBoards.Management.Models.Objects;
using KiBoards.Management.Models.Settings;
using KiBoards.Management.Models.Spaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace KiBoards.Management;

public class KibanaHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonPropertyNameCaseInsensitive = new() { PropertyNameCaseInsensitive = true };
    private readonly JsonSerializerOptions _jsonCamelCasePropertyNamingPolicy = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public KibanaHttpClient(HttpClient httpClinet)
    {
        _httpClient = httpClinet;
        _httpClient.DefaultRequestHeaders.Add("kbn-xsrf", "true");
    }

    private string GetSpaceBaseUrl(string spaceId) => !string.IsNullOrEmpty(spaceId) ? $"/s/{spaceId.ToLower()}" : string.Empty;

    /// <summary>
    /// Sets the Kibana theme (dark mode) for the default space.
    /// </summary>
    /// <param name="darkMode">Indicates whether dark mode should be enabled.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    public async Task SetDarkModeAsync(bool darkMode, CancellationToken cancellationToken = default)
        => await SetDarkModeAsync(darkMode, null, cancellationToken);

    /// <summary>
    /// Sets the Kibana theme (dark mode) for a specific space.
    /// </summary>
    /// <param name="darkMode">Indicates whether dark mode should be enabled.</param>
    /// <param name="spaceId">Optional Kibana space ID. If null, applies to the default space.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    public async Task SetDarkModeAsync(bool darkMode, string spaceId, CancellationToken cancellationToken = default)
    {
        var content = GetSettingsChangeJsonContent(new KibanaSettingsDarkMode() { ThemeDarkMode = darkMode });
        var response = await _httpClient.PostAsync($"{GetSpaceBaseUrl(spaceId)}/api/kibana/settings", content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Attempts to set the default route in Kibana for a specific space.
    /// </summary>
    /// <param name="defaultRoute">The default route to set.</param>
    /// <param name="spaceId">The space ID to apply the setting to.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>True if the request succeeded; otherwise, false.</returns>
    public async Task<bool> TrySetDefaultRoute(string defaultRoute, string spaceId, CancellationToken cancellationToken = default)
    {
        var response = await SetDefaultRoute(defaultRoute, spaceId, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<HttpResponseMessage> SetDefaultRoute(string defaultRoute, string spaceId, CancellationToken cancellationToken = default)
    {
        var content = GetSettingsChangeJsonContent(new KibanaSettingsDefaultRoute() { DefaultRoute = defaultRoute });
        return await _httpClient.PostAsync($"{GetSpaceBaseUrl(spaceId)}/api/kibana/settings", content, cancellationToken);
    }


    /// <summary>
    /// Imports saved objects from an NDJSON file into the default Kibana space.
    /// </summary>
    /// <param name="ndjsonFile">Path to the NDJSON file.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>Deserialized response from the import operation.</returns>
    public async Task<KibanaImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, CancellationToken cancellationToken = default)
        => await ImportSavedObjectsAsync(ndjsonFile, false, null, cancellationToken);

    /// <summary>
    /// Imports saved objects from an NDJSON file into a specified Kibana space.
    /// </summary>
    /// <param name="ndjsonFile">Path to the NDJSON file.</param>
    /// <param name="spaceId">The space ID to import into.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>Deserialized response from the import operation.</returns>
    public async Task<KibanaImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, string spaceId, CancellationToken cancellationToken = default)
        => await ImportSavedObjectsAsync(ndjsonFile, false, spaceId, cancellationToken);

    /// <summary>
    /// Imports saved objects from an NDJSON file into a specified space, with overwrite control.
    /// </summary>
    /// <param name="ndjsonFile">Path to the NDJSON file.</param>
    /// <param name="overwrite">Indicates whether existing objects should be overwritten.</param>
    /// <param name="spaceId">Optional Kibana space ID. If null, applies to the default space.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>Deserialized response from the import operation.</returns>
    public async Task<KibanaImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, bool overwrite, string spaceId = null, CancellationToken cancellationToken = default)
    {
        var multipartContent = new MultipartFormDataContent();
        var streamContent = new StreamContent(File.Open(ndjsonFile, FileMode.Open));
        multipartContent.Add(streamContent, "file", ndjsonFile);

        var response = await _httpClient.PostAsync($"{GetSpaceBaseUrl(spaceId)}/api/saved_objects/_import?overwrite={overwrite.ToString().ToLower()}", multipartContent);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<KibanaImportObjectsResponse>(_jsonPropertyNameCaseInsensitive, cancellationToken);

        return result;
    }

    /// <summary>
    /// Retrieves the Kibana system status.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The current Kibana system status.</returns>
    public async Task<KibanaStatusResponse> GetStatus(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<KibanaStatusResponse>("api/status", cancellationToken);

    /// <summary>
    /// Attempts to create a new Kibana space.
    /// </summary>
    /// <param name="space">The space definition to create.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>True if the space was successfully created; otherwise, false.</returns>
    public async Task<bool> TryCreateSpaceAsync(KibanaSpace space, CancellationToken cancellationToken = default)
    {
        var response = await CreateSpaceAsync(space, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<HttpResponseMessage> CreateSpaceAsync(KibanaSpace space, CancellationToken cancellationToken = default)
        => await _httpClient.PostAsJsonAsync("api/spaces/space", space, _jsonCamelCasePropertyNamingPolicy, cancellationToken);



    public async Task<bool> TryUpdateSpaceAsync(KibanaSpace space, CancellationToken cancellationToken = default)
    {
        var response = await UpdateSpaceAsync(space, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<HttpResponseMessage> UpdateSpaceAsync(KibanaSpace space, CancellationToken cancellationToken = default)
        => await _httpClient.PutAsJsonAsync($"api/spaces/space/{space.Id ?? "default"}", space, _jsonCamelCasePropertyNamingPolicy, cancellationToken);


    /// <summary>
    /// Retrieves information about a specific Kibana space.
    /// </summary>
    /// <param name="spaceId">The ID of the space to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The Kibana space object.</returns>
    public async Task<KibanaSpace> GetSpaceAsync(string spaceId, CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<KibanaSpace>($"api/spaces/space/{spaceId}", _jsonPropertyNameCaseInsensitive, cancellationToken);

    /// <summary>
    /// Creates a standardized JSON content payload for a Kibana settings change request.
    /// </summary>
    /// <typeparam name="T">The type of the settings payload.</typeparam>
    /// <param name="changes">The settings changes to apply.</param>
    /// <returns>A <see cref="JsonContent"/> object to send with the request.</returns>
    private JsonContent GetSettingsChangeJsonContent<T>(T changes) where T : class
        => JsonContent.Create(new KibanaSettingsRequest<T> { Changes = changes });
}
