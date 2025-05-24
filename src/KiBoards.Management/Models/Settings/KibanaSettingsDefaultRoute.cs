using System.Text.Json.Serialization;

namespace KiBoards.Management.Models.Settings;

internal class KibanaSettingsDefaultRoute
{

    [JsonPropertyName("defaultRoute")]
    public string DefaultRoute { get; set; } = null;

}