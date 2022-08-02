using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveSDK.Models
{
    public class MonitoringState
    {
        [JsonProperty("isLocalOutMuted")]
        public bool? IsLocalOutMuted { get; set; }

        [JsonProperty("isStreamOutMuted")]
        public bool? IsStreamOutMuted { get; set; }

        [JsonProperty("localVolumeOut")]
        public long? LocalVolumeOut { get; set; }

        [JsonProperty("streamVolumeOut")]
        public long? StreamVolumeOut { get; set; }
    }
}
