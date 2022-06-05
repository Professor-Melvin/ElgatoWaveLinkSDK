using Newtonsoft.Json;

namespace ElgatoWaveAPI.Models
{
    public class Filter
    {
        [JsonProperty("active")]
        public bool Active { get; set; }
        
        [JsonProperty("filterID")]
        public string FilterId { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("pluginID")]
        public string PluginId { get; set; }
        
    }
}