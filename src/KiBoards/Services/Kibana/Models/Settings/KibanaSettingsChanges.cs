using System.Text.Json.Serialization;

namespace KiBoards.Services
{
    public class KibanaSettingsChanges
    {
        [JsonPropertyName("theme:darkMode")]
        public bool? ThemeDarkMode { get; set; }
    }
}