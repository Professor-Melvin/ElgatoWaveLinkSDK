using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ElgatoWaveSDK.Models
{
    public class Filter
    {
        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("filterID")]
        public string? FilterId { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("pluginID")] 
        public string? PluginId { get; set; }
    }
}
