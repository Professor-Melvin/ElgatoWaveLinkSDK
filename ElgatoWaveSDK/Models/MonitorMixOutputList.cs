using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models
{
    public class MonitorMixOutputList
    {
        [JsonPropertyName("monitorMix")]
        public string? MonitorMix { get; set; }

        [JsonPropertyName("monitorMixList")]
        public List<MonitorMixList>? MonitorMixList { get; set; }
    }

    public class MonitorMixList
    {
        [JsonPropertyName("monitorMix")]
        public string? MonitorMix { get; set; }
    }
}
