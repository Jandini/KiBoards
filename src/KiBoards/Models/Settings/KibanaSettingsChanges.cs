using System.Text.Json.Serialization;

namespace KiBoards.Models.Settings
{
    class KibanaSettingsChanges
    {
        [JsonPropertyName("theme:darkMode")]
        public bool? ThemeDarkMode { get; set; } = null;

        [JsonPropertyName("defaultRoute")]
        public string DefaultRoute { get; set; } = null;

    }
}