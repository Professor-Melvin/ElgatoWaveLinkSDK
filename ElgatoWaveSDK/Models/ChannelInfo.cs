using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveSDK.Models
{
    public class ChannelInfo
    {
        [JsonProperty("bgColor")]
        public string? BgColor { get; set; }

        [JsonProperty("deltaLinked")]
        public long? DeltaLinked { get; set; }

        [JsonProperty("iconData")]
        public string? IconData { get; set; }

        [JsonProperty("inputType")]
        public long? InputType { get; set; }

        [JsonProperty("isAvailable")]
        public bool? IsAvailable { get; set; }

        [JsonProperty("isLinked")]
        public bool? IsLinked { get; set; }

        [JsonProperty("isLocalInMuted")]
        public bool? IsLocalInMuted { get; set; }

        [JsonProperty("isStreamInMuted")]
        public bool? IsStreamInMuted { get; set; }

        [JsonProperty("localVolumeIn")]
        public long? LocalVolumeIn { get; set; }

        [JsonProperty("mixId")]
        public string? MixId { get; set; }

        [JsonProperty("mixerName")]
        public string? MixerName { get; set; }

        [JsonProperty("streamVolumeIn")]
        public long? StreamVolumeIn { get; set; }
    }
}
