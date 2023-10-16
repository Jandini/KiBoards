using System.Text.Json.Serialization;

namespace KiBoards.Models.Settings
{
    class KibanaSettingsChanges
    {
        [JsonPropertyName("theme:darkMode")]
        public bool? ThemeDarkMode { get; set; }
    }
}