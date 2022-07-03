using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveSDK.Models
{
    public class MonitorMixOutputList
    {
        [JsonProperty("monitorMix")]
        public string? MonitorMix { get; set; }

        [JsonProperty("monitorMixList")]
        public List<MonitorMixList>? MonitorMixList { get; set; }
    }

    public class MonitorMixList
    {
        [JsonProperty("monitorMix")]
        public string? MonitorMix { get; set; }
    }
}
