using System.Text.Json.Serialization;

namespace KiBoards.Services
{
    class KibanaSettingsChanges
    {
        [JsonPropertyName("theme:darkMode")]
        public bool? ThemeDarkMode { get; set; }
    }
}