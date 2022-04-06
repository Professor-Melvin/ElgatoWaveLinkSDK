using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveAPI.Models
{
    public class MonitorMixOutputList
    {
        [JsonProperty("monitorMix")]
        public string MonitorMix { get; set; }

        [JsonProperty("monitorMixList")]
        public List<MonitorMixList> MonitorMixList { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }

    public class MonitorMixList
    {
        [JsonProperty("monitorMix")]
        public string MonitorMix { get; set; }
    }
}
