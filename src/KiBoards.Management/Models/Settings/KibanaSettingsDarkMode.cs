using System.Text.Json.Serialization;

namespace KiBoards.Management.Models.Settings;

internal class KibanaSettingsDarkMode
{
    [JsonPropertyName("theme:darkMode")]
    public bool? ThemeDarkMode { get; set; } = null;

}