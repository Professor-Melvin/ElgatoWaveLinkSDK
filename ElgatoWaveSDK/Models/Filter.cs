using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models
{
    public class Filter
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("filterID")]
        public string FilterId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pluginID")]
        public string PluginId { get; set; }
    }
}
